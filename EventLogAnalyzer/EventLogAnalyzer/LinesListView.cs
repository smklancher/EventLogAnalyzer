using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventLogAnalysis;

namespace EventLogAnalyzer
{
    public class LinesListView
    {
        public LinesListView(ListView lv, TextBox detail, PropertyGrid propertyGrid)
        {
            list = lv;
            list.BeginUpdate();
            list.View = View.Details;
            list.FullRowSelect = true;
            list.Columns.Add("Timestamp");
            list.Columns.Add("Level");
            list.Columns.Add("Message");
            list.VirtualMode = true;
            list.VirtualListSize = 0;
            list.RetrieveVirtualItem += List_RetrieveVirtualItem;
            list.SelectedIndexChanged += List_SelectedIndexChanged;
            list.ColumnClick += List_ColumnClick;

            DetailText = detail;
            DebugProperties = propertyGrid;

            list.EndUpdate();
        }

        public ILogEntryCollection<LogEntry> CurrentLines { get; private set; } = new LogEntryCollection<LogEntry>();

        public PropertyGrid DebugProperties { get; }

        public TextBox DetailText { get; }

        public bool SortNewestFirst { get; set; } = true;

        private ListView list { get; }

        public void SelectEarliestLine()
        {
            list.SelectedIndices.Clear();
            if (list.VirtualListSize > 0)
            {
                list.SelectedIndices.Add(ReversableIndex(0, list.VirtualListSize, SortNewestFirst));
            }
        }

        public void SelectMostRecentLine()
        {
            list.SelectedIndices.Clear();
            if (list.VirtualListSize > 0)
            {
                list.SelectedIndices.Add(ReversableIndex(list.VirtualListSize - 1, list.VirtualListSize, SortNewestFirst));
            }
        }

        public void UpdateLineSource(ILogEntryCollection<LogEntry> newlines)
        {
            list.BeginUpdate();
            //IsDisplayingInternalLog = false;
            CurrentLines = newlines;
            list.VirtualListSize = CurrentLines.Entries.Count();

            SelectMostRecentLine();

            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            list.EndUpdate();
        }

        private void List_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
            if (e.Column == 0)
            {
                list.BeginUpdate();
                SortNewestFirst = !SortNewestFirst;
                list.Invalidate();
                list.EndUpdate();
            }
        }

        private void List_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            string[] LineInfo;
            int index = ReversableIndex(e.ItemIndex, true);

            //this only makes senes knowing that it is really an IList
            // for a pure IEnumerable this will kill performance
            var Line = CurrentLines.Entries.ElementAt(index); 

            LineInfo = new string[] { TimestampOptions.ConvertToString(Line.Timestamp), Line.Level, Line.Message };

            e.Item = new ListViewItem(LineInfo);
        }

        private void List_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (list.SelectedIndices.Count < 1)
            {
                return;
            }

            int index = ReversableIndex(list.SelectedIndices[0], true);

            var Line = CurrentLines.Entries.ElementAt(index);
            DebugProperties.SelectedObject = Line;
            DetailText.Text = Line.Message;
        }

        private int ReversableIndex(int index, bool indexFromUI = false) => ReversableIndex(index, list.VirtualListSize, SortNewestFirst, indexFromUI);

        /// <summary>
        /// Return the correct index according to sorting preference (avoiding sorting the backing list)
        /// </summary>
        /// <param name="index">input index</param>
        /// <param name="count">count of the list (which should be identical to UI via VirtualListSize)</param>
        /// <param name="reverseSortPreference">sort preference that can be changed from UI</param>
        /// <param name="indexFromUI">Whether the index is from the UI (SelectedIndices,etc).  If false then expecting an index of the backing list.</param>
        /// <returns></returns>
        private int ReversableIndex(int index, int count, bool reverseSortPreference, bool indexFromUI = false)
        {
            bool reverse = reverseSortPreference ^ indexFromUI;
            return Math.Abs((Convert.ToInt32(!reverse) * (count - 1)) - index);
        }
    }
}