using Serilog;
using Serilog.Sinks.ListOfString;

namespace EventLogAnalyzer;

public class FileListView
{
    private const int ExtraItems = 2;
    private ListViewItem AllListItem;
    private ListViewItem InternalLogListItem;
    private ListViewItem[] listCache;

    public FileListView(ListView lv, LinesListView llv)
    {
        LinesList = llv;
        list = lv;
        InternalLogListItem = new ListViewItem(new[] { InternalLog.SourceName, InternalLog.TypeName, string.Empty });
        AllListItem = new ListViewItem(new[] { "All Files", "Combined", string.Empty });

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
        InternalLog.WriteLine("Ready for logs...");
        UpdateLogFilesSource(Logs);
        SelectInternalLog();
        list.EndUpdate();
    }

    public bool IsItemSelected => list.SelectedIndices.Count > 0;
    public LinesListView LinesList { get; }
    public LogCollection Logs { get; private set; }
    private int AllIndex => Logs.Count;
    private InternalLog InternalLog { get; } = InternalLog.Instance;
    private int InternalLogIndex => Logs.Count + 1;
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
        if (IsItemSelected)
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
            list.SelectedIndices.Add(InternalLogIndex);
        }
    }

    public void UpdateLogFilesSource(LogCollection logs)
    {
        list.BeginUpdate();
        Logs = logs;
        listCache = new ListViewItem[Logs.Count + ExtraItems];
        listCache[InternalLogIndex] = InternalLogListItem;
        listCache[AllIndex] = AllListItem;
        list.VirtualListSize = Logs.Count + ExtraItems;
        list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        list.EndUpdate();
    }

    private void List_RetrieveVirtualItem(object? sender, RetrieveVirtualItemEventArgs e)
    {
        var cacheItem = listCache[e.ItemIndex];
        ILogBase<LogEntry>? log = null;

        if (e.ItemIndex < Logs.Logs.Count)
        {
            log = Logs.Logs[e.ItemIndex];

            if (cacheItem is null)
            {
                var LineInfo = new string[] { log.SourceName, log.TypeName, log.LogStatus() };

                cacheItem = new ListViewItem(LineInfo);
                listCache[e.ItemIndex] = cacheItem;
            }
        }

        if (e.ItemIndex == InternalLogIndex)
        {
            log = InternalLog;
        }

        if (log is not null)
        {
            cacheItem.SubItems[2].Text = log.LogStatus();
        }

        e.Item = cacheItem;
    }

    private void List_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (IsItemSelected)
        {
            var index = list.SelectedIndices[0];
            if (index < Logs.Logs.Count)
            {
                var lines = SelectedLog().EntryCollection;
                LineSource.SetSource(() => lines);
                LinesList.RefreshFromLineSourceAndApplyFilter();
            }
            else
            {
                if (index == InternalLogIndex)
                {
                    var lines = SelectedLog().EntryCollection;
                    LineSource.SetSource(() => lines);
                    LinesList.RefreshFromLineSourceAndDontApplyFilter();
                }
                if (index == AllIndex)
                {
                    var lines = Logs.TraitTypes.Lines("All", "All");
                    LineSource.SetSource(() => lines);
                    LinesList.RefreshFromLineSourceAndApplyFilter();
                }
            }
        }
    }
}