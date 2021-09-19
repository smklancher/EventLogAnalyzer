using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EventLogAnalysis;
using Serilog;
using Serilog.Sinks.ListOfString;

namespace EventLogAnalyzer
{
    public partial class GenericLogCollectionDisplay
    {
        public List<string> InternalLog = new();
        private const string InternalLogName = "InternalLog";
        private string LastSearchText = "";
        private EventLogCollection mLogs = new();

        public GenericLogCollectionDisplay(LinesListView llv, TraitValuesListView tvlv)
        {
            Log.Logger = InternalLog.AsSeriLogger();
            LinesList = llv;
            TraitValuesList = tvlv;
        }

        public string SelectedTraitType { get; private set; } = string.Empty;

        public void DisplayFiles()
        {
            if (Logs is not null && FileList is not null)
            {
                // clear old Files
                FileList.Items.Clear();

                foreach (var Log in Logs)
                {
                    string[] LineInfo = new[] { Log.FileName, "EventLog", string.Empty };
                    ListViewItem ListLine = new ListViewItem(LineInfo);
                    FileList.Items.Add(ListLine);
                }

                FileList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        public void DisplayIndexTypes()
        {
            IndexTypeList.Items.Clear();

            var typesAndCounts = Logs.TraitTypes.TraitTypesAndCounts();

            foreach (var IdxAndCount in typesAndCounts)
            {
                IndexTypeList.Items.Add(new ListViewItem(new string[] { IdxAndCount.Count.ToString(), IdxAndCount.TypeName }));
            }

            IndexTypeList.Items.Add(new ListViewItem(new[] { "N/A", InternalLogName }));

            IndexTypeList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        public void DisplayIndices(string IndexType)
        {
            SelectedTraitType = IndexType;

            if (SelectedTraitType == InternalLogName)
            {
                DisplayInternalLog();
            }
            else
            {
                var summaries = Logs.TraitTypes.TraitValueSummaries(IndexType).OrderByDescending(x => x.Count.ToString("000000000")).ToList();
                TraitValuesList.UpdateTraitValueSummarySource(summaries);
            }
        }

        public void DisplayInternalLog()
        {
            TraitValuesList.DisplayInternalLog();
            LinesList.DisplayInternalLog(InternalLog);
        }

        public void DisplayLines(string IndexType, string IndexValue)
        {
            var newlines = Logs.TraitTypes.Lines(IndexType, IndexValue);
            if (newlines != null)
            {
                LinesList.UpdateLineSource(newlines);
            }
        }

        public void Filter(string searchText)
        {
            if (searchText.Length > LastSearchText.Length && searchText.StartsWith(LastSearchText))
            {
                //if just adding to the last search (continuing to type) then can filter from what is already showing
                LinesList.UpdateLineSource(LinesList.CurrentLines.FilteredCopy(searchText));
            }
            else
            {
                //...otherwise need to get content from the source

                EventCollection newlines;
                //messy, but this class should be refactored in general
                if (string.IsNullOrEmpty(TraitValuesList.ActiveTraitValue) && string.IsNullOrEmpty(SelectedTraitType) && mFileList.SelectedIndices.Count > 0)
                {
                    // whole file
                    newlines = Logs.Logs[mFileList.SelectedIndices[0]].FilteredEvents;
                }
                else
                {
                    // specific trait
                    newlines = Logs.TraitTypes.Lines(SelectedTraitType, TraitValuesList.ActiveTraitValue);
                }

                newlines = newlines.FilteredCopy(searchText);
                LinesList.UpdateLineSource(newlines);
            }
            LastSearchText = searchText;
        }

        public void Refresh()
        {
            DisplayFiles();
            DisplayIndexTypes();
            DisplayIndices(SelectedTraitType);
            DisplayLines(SelectedTraitType, TraitValuesList.ActiveTraitValue);
        }

        /// <summary>
        /// Set status msg... would probably be better to have the display subscribe to events
        /// </summary>
        /// <param name="Msg"></param>
        /// <remarks></remarks>
        public void SetStatus(string Msg)
        {
            StatusBar.Text = Msg;
        }

        public void StartProgressBar()
        {
            if (ProgressBar != null)
            {
                ProgressBar.Style = ProgressBarStyle.Marquee;
                ProgressBar.MarqueeAnimationSpeed = 30;
            }
        }

        public void StopProgressBar()
        {
            if (ProgressBar != null)
            {
                ProgressBar.Style = ProgressBarStyle.Continuous;
                ProgressBar.MarqueeAnimationSpeed = 0;
                ProgressBar.Value = 0;
            }
        }

        private void handleTypingTimerTimeout(object? sender, EventArgs e)
        {
            var timer = sender as Timer;

            if (timer is not null)
            {
                string SearchText = timer.Tag.ToString() ?? string.Empty;
                Filter(SearchText);
                timer.Stop();
            }
        }

        private void mFileList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (mFileList.SelectedIndices.Count < 1)
            {
                return;
            }

            //unset selected type and index
            DisplayIndices(string.Empty);

            //unset search text
            SearchBox.Text = string.Empty;

            var newlines = Logs.Logs[mFileList.SelectedIndices[0]].FilteredEvents;
            if (newlines != null)
            {
                LinesList.UpdateLineSource(newlines);
                DebugProperties.SelectedObject = Logs.Logs[mFileList.SelectedIndices[0]];
            }
        }

        private void mIndexTypeList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (IndexTypeList.SelectedItems.Count < 1)
            {
                return;
            }

            string IndexType = IndexTypeList.SelectedItems[0].SubItems[1].Text;

            //unset search text
            SearchBox.Text = string.Empty;

            // selected index is no longer valid for the new type
            SelectedTraitType = "";

            // display the new indicies
            DisplayIndices(IndexType);
        }

        private void mLogs_LogsFinishedLoading(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Refresh();
            StopProgressBar();

            //if (Logs.PreviousLinesLoaded > 0)
            //    SetStatus("Lines loaded: " + Logs.LinesLoaded - Logs.PreviousLinesLoaded + " (" + Logs.LinesLoaded + " total)");
            //else
            SetStatus($"Total events loaded: {Logs.TotalEventCount}.  Current filter events: {Logs.FilteredEventCount}");
        }

        private void mSearchBox_TextChanged(object? sender, EventArgs e)
        {
            if (mTypingTimer == null)
            {
                mTypingTimer = new Timer();
                mTypingTimer.Interval = 300;
                mTypingTimer.Tick += this.handleTypingTimerTimeout;
            }

            mTypingTimer.Stop();
            mTypingTimer.Tag = (sender as TextBox)?.Text ?? string.Empty;
            mTypingTimer.Start();
        }
    }
}