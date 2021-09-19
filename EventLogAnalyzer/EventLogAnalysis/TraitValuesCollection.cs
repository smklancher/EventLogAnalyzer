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
    /// Formerly class named Index
    /// Represents an index (such as "batches") which contains a collection of index-keys and their associated lines
    /// </summary>
    /// <remarks></remarks>

    public class TraitValuesCollection : Dictionary<string, SingleTraitValueEventCollection>, IComparable<TraitValuesCollection>
    {
        public TraitValuesCollection(string Name)
        {
            this.Name = Name;
        }

        /// <summary>
        /// This is the same as accessing the object directly.  This just makes the relationship more clear, especially on the class diagram.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public TraitValuesCollection LineCollections => this;

        /// <summary>
        /// Name of this index (such as "batch")
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string Name { get; }

        public void AddCollection(SingleTraitValueEventCollection col)
        {
            Debug.Assert(Name == col.TraitName, $"Collections should all have the same trait name: {Name} != {col.TraitName}");

            var existsInCollection = LineCollections.TryGetValue(col.TraitValue, out var lc);
            Debug.Assert(!existsInCollection, $"Added collections should not have duplicate trait values: {col.TraitValue.Substring(0, Math.Min(col.TraitValue.Length, 30))}");

            LineCollections.Add(col.TraitValue, col);
        }

        public void AddLine(string TraitValue, ELRecord Line)
        {
            var existsInCollection = LineCollections.TryGetValue(TraitValue, out var lc);
            lc ??= new SingleTraitValueEventCollection()
            {
                TraitName = Name,
                TraitValue = TraitValue,
            };

            lc.Add(Line);

            if (!existsInCollection)
            {
                LineCollections.Add(lc.TraitValue, lc);
            }
        }

        public int CompareTo(TraitValuesCollection? other)
        {
            if (other == null)
            {
                return 1;
            }

            return LineCollections.Count.CompareTo(other.Count);
        }

        /// <summary>
        /// Lines associated with the given index value
        /// </summary>
        /// <param name="IndexValue"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public EventCollection LinesFromTraitValue(string TraitValue)
        {
            LineCollections.TryGetValue(TraitValue, out var lc);
            return lc ?? new();
        }

        /// <summary>
        /// Lock the underlying lines collection, after which no more lines can be added to the index
        /// </summary>
        /// <remarks></remarks>
        public void Lock()
        {
            foreach (var lc in LineCollections.Values)
            {
                lc.Lock();
            }
        }

        public void Merge(TraitValuesCollection Other)
        {
            foreach (var OtherLines in Other.Values)
            {
                if (LineCollections.TryGetValue(OtherLines.TraitValue, out var ThisLines))
                {
                    // LineCollection with this index value already exists, so merge
                    ThisLines.Merge(OtherLines);
                }
                else
                {
                    // LineCollection wit hthis index value didn't exist, so add it diretly
                    LineCollections.Add(OtherLines.TraitValue, OtherLines);
                }
            }
        }

        /// <summary>
        /// Number of lines keyed by index value, ordered by number of lines decending
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<TraitValueSummaryLine> ValuesCounts()
        {
            return this.Select(x => new TraitValueSummaryLine(x.Key, x.Value.Count, x.Value.FirstEvent, x.Value.LastEvent)).ToList();
        }

        public record TraitValueSummaryLine(string TraitValue, long Count, DateTime? First, DateTime? Last)
        {
            public string[] AsArray(bool datesBeforeValue)
            {
                if (datesBeforeValue)
                {
                    return new string[] {
                         Count.ToString(),
                         First.ToString()?? string.Empty,
                         Last.ToString() ?? string.Empty,
                         TraitValue
                     };
                }
                else
                {
                    return new string[] {
                         Count.ToString(),
                         TraitValue,
                         First.ToString()?? string.Empty,
                         Last.ToString() ?? string.Empty
                     };
                }
            }
        }
    }
}