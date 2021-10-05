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

    // Currently traits are just a string name that gets populated somehow in code
    // Consider: Trait as definition object that defines column behavior and delegate to get value from log or log entry

    /// <summary>
    /// Formerly class named IndexCollection
    /// Collection types of traits, meaning a TraitValuesCollection for each type
    /// </summary>
    /// <remarks>Indicies keyed by the name of the index (name of index ("batch") -> index)</remarks>
    public class TraitTypeCollection : Dictionary<string, TraitValuesCollection>
    {
        private TraitTypeCollection? filteredTraitTypes;

        private DateTime? filterEnd;

        private DateTime? filterStart;

        /// <summary>
        /// This is the same as accessing the object directly.  Also will ease likely refactor from inheriting from dictionary to just containing one.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public TraitTypeCollection TraitTypes => this;

        /// <summary>
        /// Add a LogLine creating/updating Index as needed
        /// </summary>
        /// <param name="TraitName"></param>
        /// <param name="TraitValue"></param>
        /// <param name="Line"></param>
        /// <remarks></remarks>
        public void AddLine(string TraitName, string TraitValue, LogEntry Line)
        {
            var existsInCollection = TraitTypes.TryGetValue(TraitName, out var i);
            i ??= new TraitValuesCollection(TraitName);
            i.AddLine(TraitValue, Line);

            if (!existsInCollection) { TraitTypes.Add(TraitName, i); }
        }

        public void AddSingleTraitValue(SingleTraitValueEventCollection col)
        {
            var existsInCollection = TraitTypes.TryGetValue(col.TraitName, out var i);
            i ??= new TraitValuesCollection(col.TraitName);
            i.AddCollection(col);

            if (!existsInCollection) { TraitTypes.Add(col.TraitName, i); }
        }

        public void AddTraitType(TraitValuesCollection tvc)
        {
            if (TraitTypes.TryGetValue(tvc.Name, out var i))
            {
                i.Merge(tvc);
            }
            else
            {
                TraitTypes.Add(tvc.Name, tvc);
            }
        }

        /// <summary>
        /// Get lines of a given index type and value
        /// </summary>
        /// <param name="IndexType"></param>
        /// <param name="IndexValue"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public LogEntryCollection<LogEntry> Lines(string IndexType, string IndexValue)
        {
            TraitTypes.TryGetValue(IndexType, out var Idx);
            return Idx?.LinesFromTraitValue(IndexValue) ?? new LogEntryCollection<LogEntry>();
        }

        /// <summary>
        /// Lock the underlying indicies, after which no more lines can be added
        /// </summary>
        /// <remarks></remarks>
        public void Lock()
        {
            foreach (TraitValuesCollection i in TraitTypes.Values)
            {
                i.Lock();
            }
        }

        /// <summary>
        /// Merge another IndexCollection into this one
        /// </summary>
        /// <param name="Other"></param>
        /// <remarks></remarks>
        public void Merge(TraitTypeCollection Other)
        {
            // for each index in the other colection
            foreach (var OtherIndex in Other)
            {
                // need to generalize the ability of a trait provider to influence a merge of its trait, but for the moment just skip SimilarLines here
                if (OtherIndex.Key == "SimilarLines")
                {
                }
                else
                {
                    // see if it exists in this one
                    if (TraitTypes.TryGetValue(OtherIndex.Key, out var ThisIdx))
                    {
                        // if it does, merge them
                        ThisIdx.Merge(OtherIndex.Value);
                    }
                    else
                    {
                        // if it does not, then just add the index from the other to this one
                        TraitTypes.Add(OtherIndex.Key, OtherIndex.Value);
                    }
                }
            }
        }

        /// <summary>
        /// List of the index types in this collection
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<IndexTypeDisplay> TraitTypesAndCounts()
        {
            return this.Select(x => new IndexTypeDisplay(x.Key, x.Value.Count)).OrderByDescending(x => x.Count.ToString("000000000")).ToList();
        }

        public TraitValuesCollection TraitValues(string TraitType)
        {
            TraitTypes.TryGetValue(TraitType, out var TraitValues);
            return TraitValues ?? new TraitValuesCollection(TraitType);
        }

        /// <summary>
        /// List of values for a given index type
        /// </summary>
        /// <param name="TraitType"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<TraitValuesCollection.TraitValueSummaryLine> TraitValueSummaries(string TraitType)
        {
            TraitTypes.TryGetValue(TraitType, out var TraitValues);
            return TraitValues?.ValuesCounts() ?? Enumerable.Empty<TraitValuesCollection.TraitValueSummaryLine>().ToList();
        }
    }
}