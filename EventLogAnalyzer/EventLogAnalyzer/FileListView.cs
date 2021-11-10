using EventLogAnalysis;
using Serilog;
using Serilog.Sinks.ListOfString;

namespace EventLogAnalyzer
{
    public class FileListView
    {
        private ListViewItem InternalLogListItem;

        private ListViewItem[] listCache;

        public FileListView(ListView lv, LinesListView llv)
        {
            LinesList = llv;
            list = lv;
            InternalLogListItem = new ListViewItem(new[] { InternalLog.SourceName, InternalLog.TypeName, string.Empty });

            list.BeginUpdate();
            list.View = View.Details;
            list.FullRowSelect = true;
            list.Columns.Add("Source");
            list.Columns.Add("Type");
            list.Columns.Add("Status");

            list.VirtualMode = true;
            list.VirtualListSize = 0;

            list.SelectedIndexChanged += List_SelectedIndexChanged;
            list.RetrieveVirtualItem += List_RetrieveVirtualItem;

            //update to add internal log
            Logs = new LogCollection();
            listCache = new ListViewItem[0];
            Log.Logger = InternalLog.LogList.AsSeriLogger();
            Log.Information("Ready for logs...");
            UpdateLogFilesSource(Logs);
            SelectInternalLog();
            list.EndUpdate();
        }

        public InternalLog InternalLog { get; } = new();

        public bool IsLogSelected => list.SelectedIndices.Count > 0;

        public LinesListView LinesList { get; }

        public LogCollection Logs { get; private set; }

        public ILogEntryCollection<LogEntry> SelectedLogEvents => SelectedLog().EntryCollection;

        private ListView list { get; }

        public void Refresh()
        {
            list.BeginUpdate();
            list.Invalidate();
            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            list.EndUpdate();
        }

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

        public void UpdateLogFilesSource(LogCollection logs)
        {
            list.BeginUpdate();
            listCache = new ListViewItem[logs.Count + 1];
            listCache[logs.Count] = InternalLogListItem;
            Logs = logs;
            list.VirtualListSize = Logs.Logs.Count() + 1;
            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            list.EndUpdate();
        }

        private void List_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var l = e.ItemIndex > Logs.Logs.Count - 1 ? InternalLog : Logs.Logs[e.ItemIndex];
            var cacheItem = listCache[e.ItemIndex];
            if (cacheItem is null)
            {
                var LineInfo = new string[] { l.SourceName, l.TypeName, l.LogStatus() };

                cacheItem = new ListViewItem(LineInfo);
                listCache[e.ItemIndex] = cacheItem;
            }

            cacheItem.SubItems[2].Text = l.LogStatus();

            e.Item = cacheItem;
        }

        //private void List_SelectedIndexChanged(object? sender, EventArgs e)
        //{
        //    if (IsLogSelected)
        //    {
        //        LinesList.UpdateLineSource(SelectedLog().EntryCollection);
        //    }
        //}

        private void List_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (IsLogSelected)
            {
                LinesList.UpdateLineSource(SelectedLog().EntryCollection);
            }
        }
    }
}