namespace EventLogAnalysis
{
    public class LogEntryCollection<T> : ILogEntryCollection<T> where T : LogEntry
    {
        public virtual IEnumerable<T> Entries => EntryList;

        /// <summary>
        /// Presumably this isn't needed since this object itself can already cast to ILogEntryCollection<LogEntry>
        /// </summary>
        public ILogEntryCollection<LogEntry> EntriesGeneric => this;

        public DictionaryOnInsertButSortedListOnAccess<T> EntryList { get; } = new();

        // Thought this approach was needed but guess not
        // public ILogEntryCollection<LogEntry> EntriesGeneric => ((ILogEntryCollection<LogEntry>)this).AsLogEntries;

        //convenience for old code
        public IEnumerable<T> Lines => Entries;

        /// <summary>
        /// Add item by UniqueID, duplicates ignored
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            EntryList.Add(item, item.UniqueId);
        }

        public void Clear()
        {
            EntryList.Clear();
        }

        /// <summary>
        /// Merges another LineCollection into this one
        /// </summary>
        /// <param name="CollectionToMerge"></param>
        /// <remarks></remarks>
        public void Merge(LogEntryCollection<T> CollectionToMerge)
        {
            foreach (var LineToMerge in CollectionToMerge.Entries)
            {
                Add(LineToMerge);
            }
        }
    }
}