using EventLogAnalysis.Filtering;
using Microsoft.Win32;

namespace EventLogAnalyzer;

public partial class EventLogAnalyzer : Form
{
    // cts cannot be reset, so need to create new tokens per operation:
    // https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads?redirectedfrom=MSDN#operation-cancellation-versus-object-cancellation
    private CancellationTokenSource cts = new();

    private GenericLogCollectionDisplay LCD;
    private Progress<ProgressUpdate> progressHandler;

    public EventLogAnalyzer()
    {
        InitializeComponent();

        progressHandler = new Progress<ProgressUpdate>(ProgressChanged);

        LCD = new GenericLogCollectionDisplay(lstLines, lstTraitValues, lstTraitTypes, lstFiles, txtDetail, DebugProperties, ToolStripStatusLabel1, ToolStripProgressBar1, this);
    }

    public static string UNCPath(string path)
    {
        if (!path.StartsWith(@"\\"))
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey("Network\\" + path[0]))
            {
                if (key != null)
                {
                    return key.GetValue("RemotePath")?.ToString() + path.Remove(0, 2).ToString();
                }
            }
        }
        return path;
    }

    private async void EventLogAnalyzer_DragDrop(object sender, DragEventArgs e)
    {
        LCD.StartProgressBar();

        if (e.Data is not null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] MyFiles;
            MyFiles = (string[])e.Data.GetData(DataFormats.FileDrop)!;

            foreach (string File in MyFiles!)
            {
                if (Options.Instance.EnableTextLogTest)
                {
                    var el = new TextFileLog(File);
                    LCD.Logs.AddLog(el);
                }
                else
                {
                    var el = new ELog(File);
                    LCD.Logs.AddLog(el);
                }
            }

            LCD.DisplayFiles();

            await LoadAndAnalyzeAsync();
        }
    }

    private void EventLogAnalyzer_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data is not null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effect = DragDropEffects.Copy;
        }
        else
        {
            e.Effect = DragDropEffects.None;
        }
    }

    private void EventLogAnalyzer_Load(object sender, EventArgs e)
    {
        SplitDetailAndProperties.Panel2Collapsed = true;

        // enable memory/cpu status
        AppDomain.MonitoringIsEnabled = true;
        LCD.SearchBox = MessageSearchTextBox;
        LCD.ExcludeBox = SearchExcludeTextBox;
        LCD.ProgressBar = ToolStripProgressBar1;
        LCD.StatusBar = ToolStripStatusLabel1;

        //LCD.DisplayInternalLog();

        FilterOptions.InitializeFilterColumns();
    }

    private async void EventLogAnalyzer_ShownAsync(object sender, EventArgs e)
    {
        string[] args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            var file = UNCPath(args[1]);
            var el = new ELog(file);
            LCD.Logs.AddLog(el);
            LCD.DisplayFiles();
            await LoadAndAnalyzeAsync();
        }
    }

    private void filtersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        LCD.OpenFilterDialog();
    }

    private async Task LoadAndAnalyzeAsync()
    {
        LCD.StartProgressBar();
        await Task.Run(() => LCD.Logs.LoadMessages(cts.Token, LCD.StatusController.ProgressHandler));
        LCD.Refresh();

        await Task.Run(() => LCD.Logs.AnalyzeLogs(cts.Token, LCD.StatusController.ProgressHandler));
        LCD.StopProgressBar();
        LCD.Refresh();
    }

    private void localTimeEventLogDefaultToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Options.Instance.TimestampConversion = OffsetOption.ConvertToLocal;
        UpdateCheckedTimeOptions();
    }

    private void OptionsMenuItem_Click(object? sender, EventArgs e)
    {
        OptionsDialog.ShowOptions(Options.Instance, this);
    }

    private void ProgressChanged(ProgressUpdate value)
    {
        if (!string.IsNullOrWhiteSpace(value.StatusBarUpdate))
        {
            LCD.StatusBar.Text = value.StatusBarUpdate;
        }

        if (value.RefreshUI)
        {
            LCD.Refresh();
        }
    }

    private void specificLocalOffsetToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Options.Instance.TimestampConversion = OffsetOption.OffsetFromLocal;
        UpdateCheckedTimeOptions();
        OptionsDialog.ShowOptions(Options.Instance, this);
    }

    private void specificUTCOffsetToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Options.Instance.TimestampConversion = OffsetOption.OffsetFromUTC;
        UpdateCheckedTimeOptions();
        OptionsDialog.ShowOptions(Options.Instance, this);
    }

    private void toggleLineToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SplitDetailAndProperties.Panel2Collapsed = !SplitDetailAndProperties.Panel2Collapsed;
    }

    private void UpdateCheckedTimeOptions()
    {
        specificLocalOffsetToolStripMenuItem.Checked = false;
        uTCToolStripMenuItem.Checked = false;
        specificUTCOffsetToolStripMenuItem.Checked = false;
        localTimeEventLogDefaultToolStripMenuItem.Checked = false;

        switch (Options.Instance.TimestampConversion)
        {
        case OffsetOption.ConvertToLocal:
            localTimeEventLogDefaultToolStripMenuItem.Checked = true;
            break;

        case OffsetOption.UTC:
            uTCToolStripMenuItem.Checked = true;
            break;

        case OffsetOption.OffsetFromLocal:
            specificLocalOffsetToolStripMenuItem.Checked = true;
            break;

        case OffsetOption.OffsetFromUTC:
            specificUTCOffsetToolStripMenuItem.Checked = true;
            break;

        default:
            break;
        }

        // maybe just invalidate?
        LCD.Refresh();
    }

    private void uTCToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Options.Instance.TimestampConversion = OffsetOption.UTC;
        UpdateCheckedTimeOptions();
    }
}