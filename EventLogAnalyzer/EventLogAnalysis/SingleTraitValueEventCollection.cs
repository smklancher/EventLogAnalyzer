using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class SingleTraitValueEventCollection : LogEntryCollection<LogEntry>
    {
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

        public string TraitName { get; set; } = string.Empty;
        public string TraitValue { get; set; } = string.Empty;
    }
}