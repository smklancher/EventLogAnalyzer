using System;
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
    public class EventLogCollection : List<ILogBase<LogEntry>>
    {
        //convert evt to evtx: wevtutil epl application.evt application.evtx /lf:true

        //public delegate void LineParsedEventHandler(object sender, LineParsedEventArgs e);

        public delegate void LogsFinishedLoadingEventHandler(object sender, RunWorkerCompletedEventArgs e);

        //public long FilteredEventCount => Logs.Sum(x => x.FilteredEventCount);

        /// <summary>
        /// This is the same as accessing the object directly.  This just makes the relationship more clear
        /// </summary>
        public List<ILogBase<LogEntry>> Logs => this;

        //public long TotalEventCount => Logs.Sum(x => x.TotalEventCount);
        public TraitTypeCollection TraitTypes { get; private set; } = new();

        public void AddEventLogByFile(string filename)
        {
            var log = new ELog(filename);
            Logs.Add(log);

            // adding should reset or mark dirty so anything computed from existing logs is not longer valid for the new set
        }

        //public void AddByFileLoader(FileLoader loader)
        //{
        //    loader.CopyToTemp();
        //    if (loader.TempFile is not null)
        //    {
        //        AddEventLogByFile(loader.TempFile.FullName);
        //    }
        //}

        public void AnalyzeLog(ILogBase<LogEntry> log, CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
        {
            var stopwatch = Stopwatch.StartNew();
            log.LoadTraits(cancelToken);
            log.ProcessSimilarity(cancelToken);
            Log.Information($"Finished analysis of log ({stopwatch.ElapsedMilliseconds:n0} ms)"); //: {log.FileName}");
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

        //public void Filter(string xpath)
        //{
        //    foreach (var log in Logs)
        //    {
        //        log.Filter(xpath);
        //    }
        //}

        //public Dictionary<string, int> FilteredProviders()
        //{
        //    Dictionary<string, int> providers = Logs.Select(x => x.FilteredProviders).SelectMany(d => d)
        //      .GroupBy(
        //        kvp => kvp.Key,
        //        (key, kvps) => new { Key = key, Value = kvps.Sum(kvp => kvp.Value) }
        //      )
        //      .ToDictionary(x => x.Key, x => x.Value);

        //    return providers;
        //}

        /// <summary>
        /// Need to change to parallel with cancelation
        /// </summary>
        public void LoadMessages(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
        {
            var stopwatch = Stopwatch.StartNew();
            Parallel.ForEach(Logs, x => x.InitialLoad());
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
            var similarityGroups = Logs.Select(x => x.SimilarityGroups);
            var mergedSimilarityGroup = Similarity.Processing.MergeWorkingSets(similarityGroups!);
            var tvc = SimilarityConverter.FromWorkingSetGroupToTraitValuesCollection(mergedSimilarityGroup);
            TraitTypes.AddTraitType(tvc);
        }
    }

}