﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using UtilityCommon;

namespace EventLogAnalysis
{
    public class EventLogCollection : List<ELog>
    {
        //convert evt to evtx: wevtutil epl application.evt application.evtx /lf:true

        //public delegate void LineParsedEventHandler(object sender, LineParsedEventArgs e);
        //public event LineParsedEventHandler LineParsed;

        public delegate void LogsFinishedLoadingEventHandler(object sender, RunWorkerCompletedEventArgs e);

        //public event LogsFinishedLoadingEventHandler? LogsFinishedLoading;

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

        public void AnalyzeLog(ELog log, CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
        {
            var stopwatch = Stopwatch.StartNew();
            log.LoadTraits(cancelToken);
            Log.Information($"Finished analysis of log ({stopwatch.ElapsedMilliseconds:n0} ms): {log.FileName}");
        }

        public void AnalyzeLogs(CancellationToken token, IProgress<ProgressUpdate> progress)
        {
            var stopwatch = Stopwatch.StartNew();
            Parallel.ForEach(Logs, x => AnalyzeLog(x, token, progress));
            Log.Information($"Finished analysis of all logs ({stopwatch.ElapsedMilliseconds:n0} ms)");

            stopwatch.Restart();
            MergeLogAnalysis();
            Log.Information($"Finished merge of log analysis ({stopwatch.ElapsedMilliseconds:n0} ms)");

            string msg = $"Finished merged analysis of {Logs.Count} file{(Logs.Count == 1 ? string.Empty : "s")}";
            Log.Logger.Information(msg);

            progress.Report(new ProgressUpdate(true, msg));
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

        /// <summary>
        /// Need to change to parallel with cancelation
        /// </summary>
        public void LoadMessages(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
        {
            var stopwatch = Stopwatch.StartNew();
            Parallel.ForEach(Logs, x => x.LoadMessages(cancelToken, progress));
            Log.Information($"Finished loading messages of all logs ({stopwatch.ElapsedMilliseconds:n0} ms)");

            progress.Report(new ProgressUpdate(true, $"Finished loading lines of {Logs.Count} file{(Logs.Count == 1 ? string.Empty : "s")}"));
        }

        public void MergeLogAnalysis()
        {
            TraitTypes.Clear();

            foreach (var log in Logs)
            {
                // as a hack for now, this skips merging "SimilarLines"
                TraitTypes.Merge(log.Traits);
            }

            if (Options.Instance.UseNewSimilarity)
            {
                var similarityGroups = Logs.Select(x => x.SimilarityGroups);
                var mergedSimilarityGroup = Similarity.Processing.MergeWorkingSets(similarityGroups!);
                var tvc = SimilarityConverter.FromWorkingSetGroupToTraitValuesCollection(mergedSimilarityGroup);
                TraitTypes.AddTraitType(tvc);
            }
            else
            {
                MergeSimilarLines();
            }
        }

        private void MergeSimilarLines()
        {
            //now merge "Similarlines" since it was skipped above
            foreach (var log in Logs)
            {
                // compare each comparison line from one log to each in the other
                // if any comparison meets threshold, then merge
                // if not then just add new

                //The SimilarLines trait won't exist initially, so the first log will just add its SimilarLines trait to the merged collection

                bool curLogGroupAlreadyMerged = false;

                //get the trait from the current log
                if (log.Traits.TryGetValue("SimilarLines", out var CurLogSimLinesCollection))
                {
                    // Check if the merged colleciton already contains a SimilarLines trait
                    if (!TraitTypes.TryGetValue("SimilarLines", out var MergedSimLinesCollection))
                    {
                        //The SimilarLines trait won't exist initially, so the first log will just add its SimilarLines trait to the merged collection
                        TraitTypes.Add("SimilarLines", CurLogSimLinesCollection);
                    }
                    else
                    {
                        foreach (var curLogTraitValueCollection in CurLogSimLinesCollection.Values)
                        {
                            var curLogSimLinesGroup = curLogTraitValueCollection as SimilarLineGroup;
                            if (curLogSimLinesGroup is not null)
                            {
                                curLogGroupAlreadyMerged = false;

                                foreach (var mergedTraitValueCollection in MergedSimLinesCollection.Values)
                                {
                                    var mergedSimLinesGroup = mergedTraitValueCollection as SimilarLineGroup;
                                    if (mergedSimLinesGroup is not null && !curLogGroupAlreadyMerged)
                                    {
                                        if (curLogSimLinesGroup.ComparisonLine.SimilarEnough(mergedSimLinesGroup.ComparisonLine, out var similarityPercentage))
                                        {
                                            // now actually merge
                                            Log.Information($"Before merge count: {mergedSimLinesGroup.Count}");
                                            mergedSimLinesGroup.Merge(curLogSimLinesGroup);
                                            Log.Information($"Merged SimilarLines groups between logs, {similarityPercentage:P} resulting count: {mergedSimLinesGroup.Count}\r\n\r\n" +
                                                $"Merged group comparison line: \r\n{mergedSimLinesGroup.TraitValue}\r\n\r\n" +
                                                $"Current log group comparison line: \r\n{curLogSimLinesGroup.TraitValue}");
                                            curLogGroupAlreadyMerged = true;
                                        }
                                        else
                                        {
                                            Log.Verbose($"SimilarLines groups NOT merged, {similarityPercentage:P}\r\n\r\n" +
                                                $"Merged group comparison line: \r\n{mergedSimLinesGroup.TraitValue}\r\n\r\n" +
                                                $"Current log group comparison line: \r\n{curLogSimLinesGroup.TraitValue}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Currently this shouldn't happen... if later log types could not implment SimilarLines for some reason, then this might be ok
                    Log.Warning($"Log is missing SimilarLines trait: {log.FileName}");
                }
            }
        }
    }

    public record ProviderEventIdPair(string Provider, int EventId);
}