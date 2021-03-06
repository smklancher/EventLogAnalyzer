﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventLogAnalysis;
using Microsoft.VisualBasic;
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
        private EventCollection mCurrentLines = new();
        private EventLogCollection mLogs = new();
        private string mSelectedIndex = "";
        private string mSelectedIndexType = "";

        public GenericLogCollectionDisplay()
        {
            Log.Logger = InternalLog.AsSeriLogger();
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

            if (mCurrentLines != null)
            {
                DebugProperties.SelectedObject = InternalLog;
            }

            LinesList.VirtualListSize = InternalLog.Count;
            LinesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        public void DisplayLines(string IndexType, string IndexValue)
        {
            mSelectedIndex = IndexValue;
            mCurrentLines = Logs.TraitTypes.Lines(IndexType, IndexValue);
            if (mCurrentLines != null)
            {
                DebugProperties.SelectedObject = mCurrentLines;
                LinesList.VirtualListSize = mCurrentLines.Count;
                LinesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        public void Filter(string SearchText)
        {
            //messy, but this class should be refactored in general
            if (string.IsNullOrEmpty(mSelectedIndex) && string.IsNullOrEmpty(mSelectedIndexType) && mFileList.SelectedIndices.Count > 0)
            {
                // whole file
                mCurrentLines = Logs.Logs[mFileList.SelectedIndices[0]].FilteredEvents;
            }
            else
            {
                // specific trait
                mCurrentLines = Logs.TraitTypes.Lines(mSelectedIndexType, mSelectedIndex);
            }

            mCurrentLines = mCurrentLines.FilteredCopy(SearchText);
            DebugProperties.SelectedObject = mCurrentLines;
            LinesList.VirtualListSize = mCurrentLines.Count;

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

            mCurrentLines = Logs.Logs[mFileList.SelectedIndices[0]].FilteredEvents;
            if (mCurrentLines != null)
            {
                DebugProperties.SelectedObject = Logs.Logs[mFileList.SelectedIndices[0]];
            }

            LinesList.VirtualListSize = mCurrentLines?.Count ?? 0;
            LinesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void mIndexList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            switch (e.Column)
            {
            case 0:
                {
                    mCurrentIndex = mCurrentIndex.OrderByDescending(x => x.Count.ToString("000000000")).ToList();
                    break;
                }

            case 1:
                {
                    mCurrentIndex = mCurrentIndex.OrderBy(x => x.TraitValue).ToList();
                    break;
                }

            case 2:
                {
                    mCurrentIndex = mCurrentIndex.OrderBy(x => x.First).ToList();
                    break;
                }

            case 3:
                {
                    mCurrentIndex = mCurrentIndex.OrderBy(x => x.Last).ToList();
                    break;
                }
            }

            mIndexList.Invalidate();
        }

        private void mIndexList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var IndexValue = mCurrentIndex[e.ItemIndex];
            e.Item = new ListViewItem(new string[] {
            IndexValue.Count.ToString(),
            IndexValue.TraitValue,
            IndexValue.First.ToString()?? string.Empty,
            IndexValue.Last.ToString() ?? string.Empty
            });
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
                mSelectedIndex = IndexSelected;
                // display the new lines
                DisplayLines(mSelectedIndexType, mSelectedIndex);

                DebugProperties.SelectedObject = Logs.TraitTypes[mSelectedIndexType][mSelectedIndex];

                // select the first line
                mLinesList.SelectedIndices.Clear();
                if (mLinesList.VirtualListSize > 0)
                {
                    LinesList.SelectedIndices.Add(0);
                }
            }
        }

        private void mIndexTypeList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (IndexTypeList.SelectedItems.Count < 1)
            {
                return;
            }

            string IndexType = IndexTypeList.SelectedItems[0].SubItems[1].Text;

            // selected index is no longer valid for the new type
            mSelectedIndexType = "";

            // display the new indicies
            DisplayIndices(IndexType);
        }

        private void mLinesList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            string[] LineInfo;

            if (mSelectedIndexType == InternalLogName)
            {
                LineInfo = new string[] { string.Empty, "Debug", InternalLog[e.ItemIndex] };
            }
            else
            {
                var Line = mCurrentLines[e.ItemIndex] as ILogLineDisplay;
                LineInfo = new string[] { Line.TimestampString, Line.LevelString, Line.Message };
            }

            e.Item = new ListViewItem(LineInfo);
        }

        private void mLinesList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (mLinesList.SelectedIndices.Count < 1)
            {
                return;
            }

            if (mSelectedIndexType == InternalLogName)
            {
                var line = InternalLog[mLinesList.SelectedIndices[0]];
                DetailText.Text = line ?? string.Empty;
                DebugProperties.SelectedObject = null;
            }
            else
            {
                var Line = mCurrentLines[mLinesList.SelectedIndices[0]] as ILogLineDisplay;
                DebugProperties.SelectedObject = Line;
                DetailText.Text = Line.Message;
            }
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