//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Security;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.VisualBasic;

//namespace EventLogAnalysis
//{
//    public class EventCollection : LogEntryCollection<ELRecord>, IComparable<EventCollection>
//    {
//        public DictionaryOnInsertButSortedListOnAccess<ELRecord> Lines { get; } = new();

//        ///// <summary>
//        ///// Add item by UniqueID, duplicates ignored
//        ///// </summary>
//        ///// <param name="item"></param>
//        //public void Add(ELRecord item)
//        //{
//        //    Lines.Add(item, item.UniqueId);
//        //}

//        public int CompareTo(EventCollection? other)
//        {
//            if (other == null)
//            {
//                return 1;
//            }

//            // purposely using the dictionary so this doesn't need to generate the list
//            return Lines.Count.CompareTo(other.Lines.Count);
//        }

//        public EventCollection FilteredCopy(string FilterText)
//        {
//            EventCollection lc = new EventCollection();
//            foreach (var l in Lines.Where(x => x.Message.ToLower().Contains(FilterText.ToLower())))
//            {
//                lc.Add(l);
//            }

//            lc.Lines.Lock();
//            return lc;
//        }

//        ///// <summary>
//        ///// Merges another LineCollection into this one
//        ///// </summary>
//        ///// <param name="CollectionToMerge"></param>
//        ///// <remarks></remarks>
//        //public void Merge(EventCollection CollectionToMerge)
//        //{
//        //    foreach (var LineToMerge in CollectionToMerge.Lines)
//        //    {
//        //        Add(LineToMerge);
//        //    }
//        //}

//        public void SaveToFile(string FileName)
//        {
//            //StringBuilder msg = new StringBuilder();

//            //foreach (var Line in Lines)
//            //{
//            //    System.IO.FileInfo f = new System.IO.FileInfo(Line.LogFileName);

//            //    msg.AppendLine(Line.Timestamp.ToString + Constants.vbTab + Line.LevelString + Constants.vbTab + f.Name + Constants.vbTab + Line.Message);
//            //}

//            //// TODO: probably better to write line by line in a file stream
//            //My.Computer.FileSystem.WriteAllText(FileName, msg.ToString(), false);
//        }
//    }
//}