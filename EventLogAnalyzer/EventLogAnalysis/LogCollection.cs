using System.ComponentModel;
using System.Diagnostics;
using Serilog;

namespace EventLogAnalysis;

public class LogCollection : List<ILogBase<LogEntry>>
{
    //convert evt to evtx: wevtutil epl application.evt application.evtx /lf:true

    public delegate void LogsFinishedLoadingEventHandler(object sender, RunWorkerCompletedEventArgs e);

    /// <summary>
    /// This is the same as accessing the object directly.  This just makes the relationship more clear
    /// </summary>
    public List<ILogBase<LogEntry>> Logs => this;

    public TraitTypeCollection TraitTypes { get; private set; } = new();

    public void AddLog(ILogBase<LogEntry> log)
    {
        Logs.Add(log);
        // adding should reset or mark dirty so anything computed from existing logs is not longer valid for the new set
    }

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

    /// <summary>
    /// Need to change to parallel with cancelation
    /// </summary>
    public void LoadMessages(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
    {
        var stopwatch = Stopwatch.StartNew();
        Parallel.ForEach(Logs, x => x.InitialLoad(cancelToken, progress));
        Parallel.ForEach(Logs, x => x.LoadMessages(cancelToken, progress));
        Log.Information($"Finished loading messages of all logs ({stopwatch.ElapsedMilliseconds:n0} ms)");

        progress.Report(new ProgressUpdate(true, $"Finished loading lines of {Logs.Count} file{(Logs.Count == 1 ? string.Empty : "s")}") { RefreshLogsView = true });
    }

    public void MergeLogAnalysis()
    {
        TraitTypes.Clear();

        foreach (var log in Logs)
        {
            // as a hack for now, this skips merging "SimilarLines"
            TraitTypes.Merge(log.Traits);
        }
        var similarityGroups = Logs.Select(x => x.SimilarityGroups).OfType<Similarity.WorkingSetGroup<LogEntry>>();
        var mergedSimilarityGroup = Similarity.Processing.MergeWorkingSets(similarityGroups);
        var tvc = SimilarityConverter.FromWorkingSetGroupToTraitValuesCollection(mergedSimilarityGroup);
        TraitTypes.AddTraitType(tvc);
    }
}