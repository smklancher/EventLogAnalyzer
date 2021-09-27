using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventLogAnalysis;

namespace EventLogAnalyzer
{
    public class FileListView
    {
        public FileListView(ListView lv, LinesListView llv)
        {
            LinesList = llv;
            list = lv;
            list.BeginUpdate();
            list.View = View.Details;
            list.FullRowSelect = true;
            list.Columns.Add("Filename");
            list.Columns.Add("Type");

            list.SelectedIndexChanged += List_SelectedIndexChanged;
            //list.VirtualMode = true;
            //list.VirtualListSize = 0;

            list.EndUpdate();
        }

        public bool IsLogSelected => SelectedLog is null;
        public LinesListView LinesList { get; }
        public EventLogCollection Logs { get; private set; } = new();
        public ELog? SelectedLog => list.SelectedIndices.Count > 0 ? Logs.Logs[list.SelectedIndices[0]] : null;
        public EventCollection SelectedLogEvents => SelectedLog is not null ? SelectedLog.FilteredEvents : new EventCollection();
        private ListView list { get; }

        public void UpdateLogFilesSource(EventLogCollection logs)
        {
            Logs = logs;

            list.Items.Clear();

            foreach (var Log in Logs)
            {
                string[] LineInfo = new[] { Log.FileName, "EventLog", string.Empty };
                ListViewItem ListLine = new ListViewItem(LineInfo);
                list.Items.Add(ListLine);
            }

            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void List_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (SelectedLog is not null)
            {
                LinesList.UpdateLineSource(SelectedLog.FilteredEvents);
            }
        }
    }
}