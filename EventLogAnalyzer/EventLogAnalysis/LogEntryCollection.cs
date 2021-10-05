using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class LogEntryCollection<T> : ILogEntryCollection<T> where T : LogEntry
    {
        public IEnumerable<T> Entries => EntryList;
        public ILogEntryCollection<LogEntry> EntriesGeneric => ((ILogEntryCollection<LogEntry>)this).AsLogEntries;
        public ILogEntryCollection<LogEntry> EntriesGeneric2 => this;
        public DictionaryOnInsertButSortedListOnAccess<T> EntryList { get; } = new();

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