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
        private List<TraitValuesCollection.TraitValueSummaryLine> mCurrentIndex = new();

        //private EventCollection mCurrentLines = new();
        private EventLogCollection mLogs = new();

        private string mSelectedIndex = "";
        private string mSelectedIndexType = "";

        public GenericLogCollectionDisplay(LinesListView llv)
        {
            Log.Logger = InternalLog.AsSeriLogger();
            LinesList = llv;
        }

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
            mSelectedIndexType = IndexType;

            if (string.IsNullOrEmpty(IndexType))
            {
                //unselect
                IndexList.SelectedIndices.Clear();
                mSelectedIndexType = string.Empty;
                mSelectedIndex = string.Empty;
                mCurrentIndex = new();
                IndexList.VirtualListSize = 0;

                return;
            }

            if (mSelectedIndexType == InternalLogName)
            {
                DisplayInternalLog();
            }
            else
            {
                mCurrentIndex = Logs.TraitTypes.TraitValueSummaries(IndexType).OrderByDescending(x => x.Count.ToString("000000000")).ToList();
                DebugProperties.SelectedObject = mCurrentIndex;

                if (mCurrentIndex.Count > 0)
                {
                    mSelectedIndex = mCurrentIndex[0].TraitValue;
                    DisplayLines(IndexType, mSelectedIndex);
                }

                IndexList.VirtualListSize = mCurrentIndex?.Count ?? 0;
                IndexList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        public void DisplayInternalLog()
        {
            mSelectedIndex = "";
            mCurrentIndex = new();
            mIndexList.VirtualListSize = 0;

            LinesList.DisplayInternalLog(InternalLog);
        }

        public void DisplayLines(string IndexType, string IndexValue)
        {
            mSelectedIndex = IndexValue;
            var newlines = Logs.TraitTypes.Lines(IndexType, IndexValue);
            if (newlines != null)
            {
                LinesList.UpdateLineSource(newlines);
            }
        }

        public void Filter(string SearchText)
        {
            EventCollection newlines;
            //messy, but this class should be refactored in general
            if (string.IsNullOrEmpty(mSelectedIndex) && string.IsNullOrEmpty(mSelectedIndexType) && mFileList.SelectedIndices.Count > 0)
            {
                // whole file
                newlines = Logs.Logs[mFileList.SelectedIndices[0]].FilteredEvents;
            }
            else
            {
                // specific trait
                newlines = Logs.TraitTypes.Lines(mSelectedIndexType, mSelectedIndex);
            }

            newlines = newlines.FilteredCopy(SearchText);
            LinesList.UpdateLineSource(newlines);

            LastSearchText = SearchText;
        }

        public void Refresh()
        {
            DisplayFiles();
            DisplayIndexTypes();
            DisplayIndices(mSelectedIndexType);
            DisplayLines(mSelectedIndexType, mSelectedIndex);
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

        private void mIndexList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // should change to a kind of strongly typed column concept (maybe via listview extension methods)

            switch (e.Column)
            {
            case 0:
                {
                    mCurrentIndex = mCurrentIndex.OrderByDescending(x => x.Count.ToString("000000000")).ToList();
                    break;
                }

            case 1:
                {
                    if (Options.Instance.TraitDatesBeforeTraitValue)
                    {
                        mCurrentIndex = mCurrentIndex.OrderBy(x => x.First).ToList();
                    }
                    else
                    {
                        mCurrentIndex = mCurrentIndex.OrderBy(x => x.TraitValue).ToList();
                    }
                    break;
                }

            case 2:
                {
                    if (Options.Instance.TraitDatesBeforeTraitValue)
                    {
                        mCurrentIndex = mCurrentIndex.OrderBy(x => x.Last).ToList();
                    }
                    else
                    {
                        mCurrentIndex = mCurrentIndex.OrderBy(x => x.First).ToList();
                    }
                    break;
                }

            case 3:
                {
                    if (Options.Instance.TraitDatesBeforeTraitValue)
                    {
                        mCurrentIndex = mCurrentIndex.OrderBy(x => x.TraitValue).ToList();
                    }
                    else
                    {
                        mCurrentIndex = mCurrentIndex.OrderBy(x => x.Last).ToList();
                    }
                    break;
                }
            }

            mIndexList.Invalidate();
        }

        private void mIndexList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var IndexValue = mCurrentIndex[e.ItemIndex];

            if (Options.Instance.TraitDatesBeforeTraitValue)
            {
                e.Item = new ListViewItem(new string[] {
                    IndexValue.Count.ToString(),
                    IndexValue.First.ToString()?? string.Empty,
                    IndexValue.Last.ToString() ?? string.Empty,
                    IndexValue.TraitValue
                });
            }
            else
            {
                e.Item = new ListViewItem(new string[] {
                    IndexValue.Count.ToString(),
                    IndexValue.TraitValue,
                    IndexValue.First.ToString()?? string.Empty,
                    IndexValue.Last.ToString() ?? string.Empty
                });
            }
        }

        private void mIndexList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (IndexList.SelectedIndices.Count < 1)
            {
                return;
            }

            var IndexValue = mCurrentIndex[mIndexList.SelectedIndices[0]];
            string IndexSelected = IndexValue.TraitValue;

            // only do anything if we are changing the active indextype
            if (IndexSelected != mSelectedIndex)
            {
                //unset search text
                SearchBox.Text = string.Empty;

                mSelectedIndex = IndexSelected;
                // display the new lines
                DisplayLines(mSelectedIndexType, mSelectedIndex);

                DebugProperties.SelectedObject = Logs.TraitTypes[mSelectedIndexType][mSelectedIndex];

                LinesList.SelectFirstLine();
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
            mSelectedIndexType = "";

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