using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace EventLogAnalysis
{
    /// <summary>
    /// ''' Collection of indexes (batch, unique error, etc)
    /// ''' </summary>
    /// ''' <remarks>Indicies keyed by the name of the index (name of index ("batch") -> index)</remarks>
    public class IndexCollection : Dictionary<string, LogIndex>
    {
        /// <summary>
        ///     ''' This is the same as accessing the object directly.  This just makes the relationship more clear, especially on the class diagram.
        ///     ''' </summary>
        ///     ''' <value></value>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public IndexCollection Indicies
        {
            get
            {
                return this;
            }
        }

        public void AddCollection(TraitBasedEventCollection col)
        {
            var existsInCollection = Indicies.TryGetValue(col.TraitName, out var i);
            i ??= new LogIndex(col.TraitName);
            i.AddCollection(col);

            if (!existsInCollection) { Indicies.Add(col.TraitName, i); }
        }

        /// <summary>
        ///     ''' Add a LogLine creating/updating Index as needed
        ///     ''' </summary>
        ///     ''' <param name="IndexName"></param>
        ///     ''' <param name="IndexValue"></param>
        ///     ''' <param name="Line"></param>
        ///     ''' <remarks></remarks>
        public void AddLine(string IndexName, string IndexValue, ELRecord Line)
        {
            var existsInCollection = Indicies.TryGetValue(IndexName, out var i);
            i ??= new LogIndex(IndexName);
            i.AddLine(IndexValue, Line);

            if (!existsInCollection) { Indicies.Add(IndexName, i); }
        }

        /// <summary>
        ///     ''' List of the index types in this collection
        ///     ''' </summary>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public List<IndexTypeDisplay> IndexTypesAndCounts()
        {
            return this.Select(x => new IndexTypeDisplay(x.Key, x.Value.Count)).OrderByDescending(x => x.Count.ToString("000000000")).ToList();
        }

        /// <summary>
        ///     ''' List of values for a given index type
        ///     ''' </summary>
        ///     ''' <param name="IndexType"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public List<LogIndex.IndexSummaryLine> IndexValues(string IndexType)
        {
            Indicies.TryGetValue(IndexType, out var TheIndex);
            return TheIndex?.ValuesCounts() ?? Enumerable.Empty<LogIndex.IndexSummaryLine>().ToList();
        }

        /// <summary>
        ///     ''' Get lines of a given index type and value
        ///     ''' </summary>
        ///     ''' <param name="IndexType"></param>
        ///     ''' <param name="IndexValue"></param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public EventCollection Lines(string IndexType, string IndexValue)
        {
            Indicies.TryGetValue(IndexType, out var Idx);
            return Idx?.LinesFromIndexValue(IndexValue) ?? new EventCollection();
        }

        /// <summary>
        ///     ''' Lock the underlying indicies, after which no more lines can be added
        ///     ''' </summary>
        ///     ''' <remarks></remarks>
        public void Lock()
        {
            foreach (LogIndex i in Indicies.Values)
                i.Lock();
        }

        /// <summary>
        ///     ''' Merge another IndexCollection into this one
        ///     ''' </summary>
        ///     ''' <param name="Other"></param>
        ///     ''' <remarks></remarks>
        public void Merge(IndexCollection Other)
        {
            // for each index in the other colection
            foreach (var OtherIndex in Other)
            {
                // see if it exists in this one
                if (Indicies.TryGetValue(OtherIndex.Key, out var ThisIdx))
                    // if it does, merge them
                    ThisIdx.Merge(OtherIndex.Value);
                else
                    // if it does not, then just add the index from the other to this one
                    Indicies.Add(OtherIndex.Key, OtherIndex.Value);
            }
        }

        public class IndexTypeDisplay
        {
            public IndexTypeDisplay(string TypeName, int Count)
            {
                this.TypeName = TypeName;
                this.Count = Count;
            }

            public int Count { get; set; }
            public string TypeName { get; set; } = "";
        }
    }
}