using EventLogAnalysis;

namespace EventLogAnalyzer
{
    public class StatusController
    {
        public StatusController(ToolStripStatusLabel statusLabel, ToolStripProgressBar progressBar, FileListView fileList)
        {
            StatusLabel = statusLabel;
            StatusLabel.Alignment = ToolStripItemAlignment.Left;
            StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            StatusLabel.Spring = true;

            ProgressBar = progressBar;
            ProgressBar.Style = ProgressBarStyle.Marquee;
            ProgressBar.MarqueeAnimationSpeed = 0;

            FileList = fileList;

            ProgressHandler = new Progress<ProgressUpdate>(ProgressChanged);
        }

        public FileListView FileList { get; private set; }

        public ToolStripProgressBar ProgressBar { get; private set; }
        public Progress<ProgressUpdate> ProgressHandler { get; private set; }
        public ToolStripStatusLabel StatusLabel { get; private set; }

        public void StartProgressBar()
        {
            ProgressBar.Style = ProgressBarStyle.Marquee;
            ProgressBar.MarqueeAnimationSpeed = 30;
        }

        public void StopProgressBar()
        {
            ProgressBar.Style = ProgressBarStyle.Continuous;
            ProgressBar.MarqueeAnimationSpeed = 0;
            ProgressBar.Value = 0;
        }

        private void ProgressChanged(ProgressUpdate value)
        {
            if (!string.IsNullOrWhiteSpace(value.StatusBarUpdate))
            {
                StatusLabel.Text = value.StatusBarUpdate;
            }

            if (value.RefreshUI)
            {
                //LCD.Refresh();
            }

            if (value.RefreshLogsView)
            {
                FileList.Refresh();
            }
        }
    }
}