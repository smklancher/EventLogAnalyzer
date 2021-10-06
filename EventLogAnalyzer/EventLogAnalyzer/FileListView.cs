using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventLogAnalysis;
using Serilog;
using Serilog.Sinks.ListOfString;

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
            list.Columns.Add("Source");
            list.Columns.Add("Type");

            list.SelectedIndexChanged += List_SelectedIndexChanged;

            //update to add internal log
            Logs = new EventLogCollection();
            Log.Logger = InternalLog.LogList.AsSeriLogger();
            Log.Information("Ready for logs...");
            UpdateLogFilesSource(Logs);
            SelectInternalLog();
            list.EndUpdate();
        }

        public InternalLog InternalLog { get; } = new();

        public bool IsLogSelected => list.SelectedIndices.Count > 0;

        public LinesListView LinesList { get; }

        public EventLogCollection Logs { get; private set; }

        public LogEntryCollection<LogEntry> SelectedLogEvents => (LogEntryCollection<LogEntry>)SelectedLog().EntryCollection;

        private ListView list { get; }

        public ILogBase<LogEntry> SelectedLog()
        {
            if (IsLogSelected)
            {
                var index = list.SelectedIndices[0];

                if (index < Logs.Count)
                {
                    return Logs.Logs[index];
                }
            }

            // if beyond bounds of list, return internal log
            return InternalLog;
        }

        public void SelectInternalLog()
        {
            list.SelectedIndices.Clear();
            if (list.Items.Count > 0)
            {
                list.SelectedIndices.Add(list.Items.Count - 1);
            }
        }

        public void UpdateLogFilesSource(EventLogCollection logs)
        {
            Logs = logs;

            list.Items.Clear();

            foreach (var l in Logs)
            {
                string[] LineInfo = new[] { l.SourceName, l.TypeName, string.Empty };
                ListViewItem ListLine = new ListViewItem(LineInfo);
                list.Items.Add(ListLine);
            }

            // internal log at end of list
            list.Items.Add(new ListViewItem(new[] { InternalLog.SourceName, InternalLog.TypeName, string.Empty }));

            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void List_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (IsLogSelected)
            {
                LinesList.UpdateLineSource(SelectedLog().EntryCollection);
            }
        }
    }
}