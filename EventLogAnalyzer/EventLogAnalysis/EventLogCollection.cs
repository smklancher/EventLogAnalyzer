using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using UtilityCommon;

namespace EventLogAnalysis
{
    public class EventLogCollection : List<ELog>
    {
        //convert evt to evtx: wevtutil epl application.evt application.evtx /lf:true

        //public delegate void LineParsedEventHandler(object sender, LineParsedEventArgs e);
        //public event LineParsedEventHandler LineParsed;

        public delegate void LogsFinishedLoadingEventHandler(object sender, RunWorkerCompletedEventArgs e);

        public event LogsFinishedLoadingEventHandler? LogsFinishedLoading;

        public Dictionary<ProviderEventIdPair, EventIdGroup> EventIdGroups { get; private set; } = new();

        public long FilteredEventCount => Logs.Sum(x => x.FilteredEventCount);

        /// <summary>
        /// This is the same as accessing the object directly.  This just makes the relationship more clear
        /// </summary>
        public List<ELog> Logs => this;

        public long TotalEventCount => Logs.Sum(x => x.TotalEventCount);
        public TraitTypeCollection TraitTypes { get; private set; } = new();

        /// <summary>
        /// Index collections from each log as it finishes analyzing, to be merged.
        /// Since these are not tied to the log after adding, probably not the best way to handle this.
        /// </summary>
        //private ConcurrentBag<TraitTypeCollection> SeparateTraitTypeCollections { get; } = new();

        public void AddByFile(string filename)
        {
            var log = new ELog(filename);
            Logs.Add(log);

            // adding should reset or mark dirty so anything computed from existing logs is not longer valid for the new set
        }

        public void AddByFileLoader(FileLoader loader)
        {
            loader.CopyToTemp();
            if (loader.TempFile is not null)
            {
                AddByFile(loader.TempFile.FullName);
            }
        }

        public void AnalyzeLog(ELog log, CancellationToken cancelToken, IProgress<string> progress)
        {
            log.LoadTraits(cancelToken);
            //SeparateTraitTypeCollections.Add(log.Traits);
        }

        public void AnalyzeLogs(CancellationToken token, IProgress<string> progress)
        {
            foreach (var l in Logs)
            {
                AnalyzeLog(l, token, progress);
            }

            MergeLogAnalysis();

            //LogsFinishedLoading?.Invoke(this, new RunWorkerCompletedEventArgs(null, null, false));
            Log.Logger.Information($"Finished analysis of {Logs.Count} file{(Logs.Count == 1 ? string.Empty : "s")}");
        }

        public void Filter(string xpath)
        {
            foreach (var log in Logs)
            {
                log.Filter(xpath);
            }
        }

        public Dictionary<string, int> FilteredProviders()
        {
            Dictionary<string, int> providers = Logs.Select(x => x.FilteredProviders).SelectMany(d => d)
              .GroupBy(
                kvp => kvp.Key,
                (key, kvps) => new { Key = key, Value = kvps.Sum(kvp => kvp.Value) }
              )
              .ToDictionary(x => x.Key, x => x.Value);

            return providers;
        }

        public void MergeLogAnalysis()
        {
            TraitTypes.Clear();

            foreach (var log in Logs)
            {
                // as a hack for now, this skips merging "SimilarLines"
                TraitTypes.Merge(log.Traits);
            }

            //now merge "Similarlines" since it was skipped above
            foreach (var log in Logs)
            {
                // compare each comparison line from one log to each in the other
                // if any comparison meets threshold, then merge
                // if not then just add new

                //get the trait from the current log
                if (log.Traits.TryGetValue("SimilarLines", out var CurLogSimLinesCollection))
                {
                    // see if it exists in this one (the merged results)
                    if (TraitTypes.TryGetValue("SimilarLines", out var MergedSimLinesCollection))
                    {
                        // if it does, merge them
                        //MergedSimLinesCollection.Merge(CurLogSimLinesCollection.Value);

                        foreach (var curLogTraitValueCollection in CurLogSimLinesCollection.Values)
                        {
                            var curLogSimLinesGroup = curLogTraitValueCollection as SimilarLineGroup;
                            if (curLogSimLinesGroup is not null)
                            {
                                foreach (var mergedTraitValueCollection in MergedSimLinesCollection.Values)
                                {
                                    var mergedSimLinesGroup = mergedTraitValueCollection as SimilarLineGroup;
                                    if (mergedSimLinesGroup is not null)
                                    {
                                        if (curLogSimLinesGroup.ComparisonLine.SimilarEnough(mergedSimLinesGroup.ComparisonLine))
                                        {
                                            // now actually merge
                                            mergedSimLinesGroup.Merge(curLogSimLinesGroup);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // if it does not, then just add the index from the other to this one
                        TraitTypes.Add("SimilarLines", CurLogSimLinesCollection);
                    }
                }
            }
        }

        /// <summary>
        /// Need to change to parallel with cancelation
        /// </summary>
        public void SimpleAnalyzeLogs()
        {
            var cancelToken = new CancellationTokenSource();
            var progress = new Progress<string>();
            foreach (var l in Logs)
            {
                AnalyzeLog(l, cancelToken.Token, progress);
            }

            MergeLogAnalysis();

            //LogsFinishedLoading?.Invoke(this, new RunWorkerCompletedEventArgs(null, null, false));
        }

        /// <summary>
        /// Need to change to parallel with cancelation
        /// Populates EventIdGroups, and groups similar lines within
        /// </summary>
        //public void SimpleGroupSimilarLines()
        //{
        //    EventIdGroups = CreateEventIdGroups(Logs);
        //    GroupSimilarLines(EventIdGroups);

        //    /// need to decide if this stuff is happening:
        //    /// per log - better for simple parallel performance, but less exacting - and then indexes merged with custom merge comparing applicable groups
        //    /// at the log collection level - more exact (can still paralellize per log?) - after indexes merge
        //}

        /// <summary>
        /// Need to change to parallel with cancelation
        /// </summary>
        public void SimpleLoadMessages()
        {
            var cancelToken = new CancellationTokenSource();
            foreach (var log in Logs)
            {
                log.LoadMessages(cancelToken.Token);
            }

            LogsFinishedLoading?.Invoke(this, new RunWorkerCompletedEventArgs(null, null, false));
            Log.Logger.Information($"Finished loading lines of {Logs.Count} file{(Logs.Count == 1 ? string.Empty : "s")}");
        }

        /// <summary>
        /// Need to change to parallel with cancelation
        /// Groups similar lines within the EventIdGroups
        /// </summary>
        /// <param name="LineGroupings"></param>
        //private static void GroupSimilarLines(Dictionary<ProviderEventIdPair, EventIdGroup> LineGroupings)
        //{
        //    using var dt = new DisposableTrace();
        //    foreach (var g in LineGroupings.Values)
        //    {
        //        g.GroupSimilarLines();
        //    }
        //}
    }

    public record ProviderEventIdPair(string Provider, int EventId);
}