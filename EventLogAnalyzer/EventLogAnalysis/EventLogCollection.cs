using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public IndexCollection IndexCollection { get; private set; } = new();

        /// <summary>
        /// This is the same as accessing the object directly.  This just makes the relationship more clear
        /// </summary>
        public List<ELog> Logs => this;

        public long TotalEventCount => Logs.Sum(x => x.TotalEventCount);

        /// <summary>
        /// Index collections from each log as it finishes analyzing, to be merged.
        /// Since these are not tied to the log after adding, probably not the best way to handle this.
        /// </summary>
        private ConcurrentBag<IndexCollection> SeparateIndexCollections { get; } = new();

        /// <summary>
        /// Add all lines into groups for that event type
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        //public static Dictionary<ProviderEventIdPair, EventIdGroup> CreateEventIdGroups(List<ELog> logs)
        //{
        //    using var dt = new DisposableTrace();

        //    Dictionary<ProviderEventIdPair, EventIdGroup> groups = new();
        //    foreach (var line in logs.SelectMany(x => x.AllEvents))
        //    {
        //        if (!groups.ContainsKey(line.GroupKey))
        //        {
        //            // first of this kind of event, so create a new line group
        //            var group = new EventIdGroup(line.GroupKey);
        //            groups.Add(group.GroupKey, group);
        //        }

        //        groups[line.GroupKey].Records.Add(line);
        //    }
        //    return groups;
        //}

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
            log.LoadIndicies(cancelToken);
            SeparateIndexCollections.Add(log.IndexCollection);
        }

        public void AnalyzeLogs(CancellationToken token, IProgress<string> progress)
        {
            foreach (var l in Logs)
            {
                AnalyzeLog(l, token, progress);
            }

            MergeLogAnalysis();

            //LogsFinishedLoading?.Invoke(this, new RunWorkerCompletedEventArgs(null, null, false));
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
            foreach (var idxcol in SeparateIndexCollections)
            {
                IndexCollection.Merge(idxcol);
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