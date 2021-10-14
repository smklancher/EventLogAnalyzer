using System;
using System.Diagnostics;
using System.Threading;
using Serilog;
using Similarity;

namespace EventLogAnalysis
{
    public class LogBase<T> : ILogBase<T> where T : LogEntry
    {
        public virtual ILogEntryCollection<T> EntryCollection { get; } = new LogEntryCollection<T>();

        public Guid LogGuid { get; } = new();

        public WorkingSetGroup<LogEntry>? SimilarityGroups { get; protected set; }
        public string SourceName { get; protected set; } = "UnknownLogSouce";
        public TraitTypeCollection Traits { get; protected set; } = new();
        public string TypeName { get; protected set; } = nameof(LogBase<T>);

        public virtual void InitialLoad(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
        {
        }

        public virtual void LoadMessages(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
        {
        }

        public virtual void LoadTraits(CancellationToken cancelToken)
        {
        }

        public virtual string LogStatus() => string.Empty;

        public virtual void ProcessSimilarity(CancellationToken cancelToken)
        {
            // create index for similar lines, ideally standardize index approach
            SimilarityGroups = Processing.Process(
                EntryCollection.AsLogEntries.Entries,
                Options.Instance.SimilarityOptions.LinesPerSimilarityGroupChunk,
                x => string.IsNullOrWhiteSpace(x.ShortMessage) ? x.Message : x.ShortMessage);
        }
    }
}