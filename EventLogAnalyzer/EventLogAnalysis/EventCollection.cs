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
    public class EventCollection : DictionaryOnInsertButSortedListOnAccess<ELRecord>, IComparable<EventCollection>
    {
        /// <summary>
        /// Returns the date of the first event in the collection... currently this causes a sort (costly))
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime FirstEvent => Lines.FirstOrDefault()?.Timestamp ?? DateTime.MinValue;

        /// <summary>
        /// Returns the date of the last event in the collection... currently this causes a sort (costly))
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime LastEvent => Lines.LastOrDefault()?.Timestamp ?? DateTime.MaxValue;

        /// <summary>
        /// This is the same as accessing the object directly.  This just makes the relationship more clear, especially on the class diagram.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public EventCollection Lines
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Add item by UniqueID, duplicates ignored
        /// </summary>
        /// <param name="item"></param>
        public override void Add(ELRecord item)
        {
            base.Add(item, item.UniqueId);
        }

        public int CompareTo(EventCollection? other)
        {
            if (other == null)
            {
                return 1;
            }

            // purposely using the dictionary so this doesn't need to generate the list
            return Count.CompareTo(other.Count);
        }

        public EventCollection FilteredCopy(string FilterText)
        {
            EventCollection lc = new EventCollection();
            foreach (var l in Lines.Where(x => x.Message.ToLower().Contains(FilterText.ToLower())))
            {
                lc.Add(l);
            }

            lc.Lock();
            return lc;
        }

        /// <summary>
        /// Merges another LineCollection into this one
        /// </summary>
        /// <param name="CollectionToMerge"></param>
        /// <remarks></remarks>
        public void Merge(EventCollection CollectionToMerge)
        {
            foreach (var LineToMerge in CollectionToMerge)
            {
                Add(LineToMerge);
            }
        }

        public void SaveToFile(string FileName)
        {
            //StringBuilder msg = new StringBuilder();

            //foreach (var Line in Lines)
            //{
            //    System.IO.FileInfo f = new System.IO.FileInfo(Line.LogFileName);

            //    msg.AppendLine(Line.Timestamp.ToString + Constants.vbTab + Line.LevelString + Constants.vbTab + f.Name + Constants.vbTab + Line.Message);
            //}

            //// TODO: probably better to write line by line in a file stream
            //My.Computer.FileSystem.WriteAllText(FileName, msg.ToString(), false);
        }
    }
}