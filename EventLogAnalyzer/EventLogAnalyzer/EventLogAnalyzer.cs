using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventLogAnalysis;
using Microsoft.Win32;

namespace EventLogAnalyzer
{
    public partial class EventLogAnalyzer : Form
    {
        private GenericLogCollectionDisplay LCD = new GenericLogCollectionDisplay();
        private EventLogCollection Logs = new();

        public EventLogAnalyzer()
        {
            InitializeComponent();
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

        private void EventLogAnalyzer_DragDrop(object sender, DragEventArgs e)
        {
            LCD.StartProgressBar();

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] MyFiles;
                MyFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string File in MyFiles)
                    Logs.AddByFile(File);

                LoadAndAnalyze();
            }
        }

        private void EventLogAnalyzer_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void EventLogAnalyzer_Load(object sender, EventArgs e)
        {
            // enable memory/cpu status
            AppDomain.MonitoringIsEnabled = true;

            LCD.IndexTypeList = lstIndexType;
            LCD.IndexList = lstIndex;
            LCD.LinesList = lstLines;
            LCD.FileList = lstFiles;
            LCD.DetailText = txtDetail;
            LCD.SearchBox = MessageSearchTextBox;
            LCD.DebugProperties = DebugProperties;
            LCD.ProgressBar = ToolStripProgressBar1;
            LCD.StatusBar = ToolStripStatusLabel1;
            LCD.Logs = Logs;

            //LCD.DisplayInternalLog();
        }

        private void EventLogAnalyzer_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Logs.AddByFile(UNCPath(args[1]));
                LoadAndAnalyze();
            }
        }

        private void LoadAndAnalyze()
        {
            Logs.SimpleLoadMessages();
            //Logs.SimpleGroupSimilarLines();
            Logs.SimpleAnalyzeLogs();
        }
    }
}