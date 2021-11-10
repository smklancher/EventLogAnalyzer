using Similarity;

namespace EventLogAnalysis
{
    public interface ILogBase<out T> where T : LogEntry
    {
        public ILogEntryCollection<T> EntryCollection { get; }

        public Guid LogGuid { get; }

        public WorkingSetGroup<LogEntry>? SimilarityGroups { get; }
        public string SourceName { get; }
        public TraitTypeCollection Traits { get; }
        public string TypeName { get; }

        public void InitialLoad(CancellationToken cancelToken, IProgress<ProgressUpdate> progress);

        public void LoadMessages(CancellationToken cancelToken, IProgress<ProgressUpdate> progress);

        public void LoadTraits(CancellationToken cancelToken);

        public string LogStatus();

        public void ProcessSimilarity(CancellationToken cancelToken);
    }
}