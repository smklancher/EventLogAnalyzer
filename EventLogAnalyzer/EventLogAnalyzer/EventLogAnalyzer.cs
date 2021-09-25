using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventLogAnalysis;
using Microsoft.Win32;

namespace EventLogAnalyzer
{
    public partial class EventLogAnalyzer : Form
    {
        // cts cannot be reset, so need to create new tokens per operation:
        // https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads?redirectedfrom=MSDN#operation-cancellation-versus-object-cancellation
        private CancellationTokenSource cts = new();

        private GenericLogCollectionDisplay LCD;
        private EventLogCollection Logs = new();
        private Progress<ProgressUpdate> progressHandler;

        public EventLogAnalyzer()
        {
            InitializeComponent();

            progressHandler = new Progress<ProgressUpdate>(ProgressChanged);

            var linesList = new LinesListView(lstLines, txtDetail, DebugProperties);
            var traitValuesList = new TraitValuesListView(lstIndex, linesList, DebugProperties);
            LCD = new GenericLogCollectionDisplay(linesList, traitValuesList);
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

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] MyFiles;
                MyFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string File in MyFiles)
                {
                    Logs.AddByFile(File);
                }

                LCD.DisplayFiles();

                await LoadAndAnalyzeAsync();
            }
        }

        private void EventLogAnalyzer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
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
            LCD.IndexTypeList = lstIndexType;
            LCD.FileList = lstFiles;
            LCD.DetailText = txtDetail;
            LCD.SearchBox = MessageSearchTextBox;
            LCD.DebugProperties = DebugProperties;
            LCD.ProgressBar = ToolStripProgressBar1;
            LCD.StatusBar = ToolStripStatusLabel1;
            LCD.Logs = Logs;

            //LCD.DisplayInternalLog();
        }

        private async void EventLogAnalyzer_ShownAsync(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Logs.AddByFile(UNCPath(args[1]));
                LCD.DisplayFiles();
                await LoadAndAnalyzeAsync();
            }
        }

        private async Task LoadAndAnalyzeAsync()
        {
            LCD.StartProgressBar();
            await Task.Run(() => Logs.LoadMessages(cts.Token, progressHandler));
            LCD.Refresh();

            await Task.Run(() => Logs.AnalyzeLogs(cts.Token, progressHandler));
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
            if (!string.IsNullOrWhiteSpace(value.StatusBarText))
            {
                LCD.StatusBar.Text = value.StatusBarText;
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
}