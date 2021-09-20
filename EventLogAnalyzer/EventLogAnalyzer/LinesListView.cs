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
        private List<string> internalLog = new();

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

            DetailText = detail;
            DebugProperties = propertyGrid;

            list.EndUpdate();
        }

        public EventCollection CurrentLines { get; private set; } = new();

        public PropertyGrid DebugProperties { get; }

        public TextBox DetailText { get; }

        public bool IsDisplayingInternalLog { get; private set; } = false;

        private ListView list { get; }

        public void DisplayInternalLog(List<string> internalLog)
        {
            if (internalLog is null) { return; }
            DebugProperties.SelectedObject = this.internalLog;

            list.BeginUpdate();

            IsDisplayingInternalLog = true;
            this.internalLog = internalLog;
            list.VirtualListSize = this.internalLog.Count;

            SelectLineLine();

            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            list.EndUpdate();
        }

        public void SelectFirstLine()
        {
            list.SelectedIndices.Clear();
            if (list.VirtualListSize > 0)
            {
                list.SelectedIndices.Add(0);
            }
        }

        public void SelectLineLine()
        {
            list.SelectedIndices.Clear();
            if (list.VirtualListSize > 0)
            {
                list.SelectedIndices.Add(list.VirtualListSize - 1);
            }
        }

        public void UpdateLineSource(EventCollection newlines)
        {
            list.BeginUpdate();
            IsDisplayingInternalLog = false;
            CurrentLines = newlines;
            list.VirtualListSize = CurrentLines.Count;

            SelectFirstLine();

            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            list.EndUpdate();
        }

        private void List_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            string[] LineInfo;

            if (IsDisplayingInternalLog)
            {
                LineInfo = new string[] { string.Empty, "Debug", internalLog[e.ItemIndex] };
            }
            else
            {
                var Line = CurrentLines[e.ItemIndex];
                LineInfo = new string[] { TimestampOptions.ConvertToString(Line.Timestamp), Line.Level, Line.Message };
            }

            e.Item = new ListViewItem(LineInfo);
        }

        private void List_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (list.SelectedIndices.Count < 1)
            {
                return;
            }

            if (IsDisplayingInternalLog)
            {
                var line = internalLog[list.SelectedIndices[0]];
                DetailText.Text = line ?? string.Empty;
                DebugProperties.SelectedObject = null;
            }
            else
            {
                var Line = CurrentLines[list.SelectedIndices[0]];
                DebugProperties.SelectedObject = Line;
                DetailText.Text = Line.Message;
            }
        }
    }
}