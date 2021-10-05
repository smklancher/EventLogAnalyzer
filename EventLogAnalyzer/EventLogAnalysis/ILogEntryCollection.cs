using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// might need to make this an interface to take advangage of generic covariance (not supported for classes)
// ILogEntryCollection<out T> where T : LogEntry
// then a function could return an EventCollction (ILogEntryCollection<ELRecord>) as ILogEntryCollection<LogEntry> to use generically in UI, etc.
// And Entries becomes IEnumerable for covariance as well

namespace EventLogAnalysis
{
    public interface ILogEntryCollection<out T> where T : LogEntry
    {
        public ILogEntryCollection<LogEntry> AsLogEntries => this;

        public IEnumerable<T> Entries { get; }

        /// <summary>
        /// Returns the date of the first event in the collection... currently this causes a sort (costly))
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual DateTime FirstEvent => Entries.FirstOrDefault()?.Timestamp ?? DateTime.MinValue;

        /// <summary>
        /// Returns the date of the last event in the collection... currently this causes a sort (costly))
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual DateTime LastEvent => Entries.LastOrDefault()?.Timestamp ?? DateTime.MaxValue;


        public virtual ILogEntryCollection<T> FilteredCopy(string FilterText)
        {
            var lc = new LogEntryCollection<T>();
            foreach (var l in Entries.Where(x => x.Message.ToLower().Contains(FilterText.ToLower())))
            {
                lc.Add(l);
            }

            lc.EntryList.Lock();
            return lc;
        }
    }
}