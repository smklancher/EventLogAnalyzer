using EventLogAnalysis;
using Microsoft.VisualBasic;
using System;
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

namespace EventLogAnalyzer
{
    public class GenericLogCollectionDisplay
    {
        private const string InternalLogName = "InternalLog";
        private string LastSearchText = "";
        private List<LogIndex.IndexSummaryLine> mCurrentIndex = new();
        private EventCollection mCurrentLines = new();
        private PropertyGrid mDebugProperties = new();
        private TextBox mDetailText = new();
        private ListView mFileList = new();
        private ListView mIndexList = new();
        private ListView mIndexTypeList = new();
        private ListView mLinesList = new();
        private EventLogCollection mLogs = new();
        private ToolStripProgressBar mProgressBar = new();
        private TextBox mSearchBox = new();
        private string mSelectedIndex = "";
        private string mSelectedIndexType = "";
        private ToolStripStatusLabel mStatusBar = new();
        private Timer mTypingTimer = new();

        public PropertyGrid DebugProperties
        {
            get
            {
                return mDebugProperties;
            }
            set
            {
                value.ToolbarVisible = false;
                mDebugProperties = value;
            }
        }

        public TextBox DetailText { get; set; } = new();

        public ListView FileList
        {
            set
            {
                if (mFileList is not null)
                {
                    mFileList.SelectedIndexChanged -= mFileList_SelectedIndexChanged;
                }

                value.View = View.Details;
                value.FullRowSelect = true;
                value.Columns.Add("Filename");
                value.Columns.Add("Type");
                mFileList = value;

                mFileList.SelectedIndexChanged += mFileList_SelectedIndexChanged;
            }
            get
            {
                return mFileList;
            }
        }

        public ListView IndexList
        {
            set
            {
                if (mIndexList is not null)
                {
                    mIndexList.RetrieveVirtualItem -= mIndexList_RetrieveVirtualItem;
                    mIndexList.SelectedIndexChanged -= mIndexList_SelectedIndexChanged;
                }

                value.View = View.Details;
                value.FullRowSelect = true;
                value.Columns.Add("#");
                value.Columns.Add("Indicies");
                value.Columns.Add("First");
                value.Columns.Add("Last");
                value.VirtualMode = true;
                value.VirtualListSize = 0;
                mIndexList = value;

                mIndexList.RetrieveVirtualItem += mIndexList_RetrieveVirtualItem;
                mIndexList.SelectedIndexChanged += mIndexList_SelectedIndexChanged;
            }
            get
            {
                return mIndexList;
            }
        }

        public ListView IndexTypeList
        {
            set
            {
                if (mIndexTypeList is not null)
                {
                    mIndexTypeList.SelectedIndexChanged -= mIndexTypeList_SelectedIndexChanged;
                }
                value.View = View.Details;
                value.FullRowSelect = true;
                value.Columns.Add("#");
                value.Columns.Add("Index Types");
                mIndexTypeList = value;

                mIndexTypeList.SelectedIndexChanged += mIndexTypeList_SelectedIndexChanged;
            }
            get
            {
                return mIndexTypeList;
            }
        }

        public ListView LinesList
        {
            set
            {
                if (mLinesList is not null)
                {
                    mLinesList.RetrieveVirtualItem -= mLinesList_RetrieveVirtualItem;
                    mLinesList.SelectedIndexChanged -= mLinesList_SelectedIndexChanged;
                }

                value.View = View.Details;
                value.FullRowSelect = true;
                value.Columns.Add("Timestamp");
                value.Columns.Add("Level");
                value.Columns.Add("Message");
                value.VirtualMode = true;
                value.VirtualListSize = 0;
                mLinesList = value;

                mLinesList.RetrieveVirtualItem += mLinesList_RetrieveVirtualItem;
                mLinesList.SelectedIndexChanged += mLinesList_SelectedIndexChanged;
            }
            get
            {
                return mLinesList;
            }
        }

        public EventLogCollection Logs
        {
            get
            {
                if (mLogs == null)
                    mLogs = new EventLogCollection();
                return mLogs;
            }
            set
            {
                if (mLogs is not null)
                {
                    mLogs.LogsFinishedLoading -= mLogs_LogsFinishedLoading;
                }

                mCurrentIndex = new();
                mLogs = value;

                mLogs.LogsFinishedLoading += mLogs_LogsFinishedLoading;
            }
        }

        public ToolStripProgressBar ProgressBar
        {
            get
            {
                return mProgressBar;
            }
            set
            {
                value.Style = ProgressBarStyle.Marquee;
                value.MarqueeAnimationSpeed = 0;
                mProgressBar = value;
            }
        }

        public TextBox SearchBox { get; set; } = new();

        public ToolStripStatusLabel StatusBar
        {
            set
            {
                mStatusBar = value;
            }
            get
            {
                return mStatusBar;
            }
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

            var providers = Logs.FilteredProviders();
            var typesAndCounts = Logs.IndexCollection.IndexTypesAndCounts();

            foreach (var IdxAndCount in typesAndCounts)
            {
                IndexTypeList.Items.Add(new ListViewItem(new string[] { IdxAndCount.Count.ToString(), IdxAndCount.TypeName }));
            }

            IndexTypeList.Items.Add(new ListViewItem(new[] { "N/A", InternalLogName }));

            IndexTypeList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        public void DisplayIndices(string Index)
        {
            if (Index == InternalLogName)
                DisplayInternalLog();
            else
            {
                mCurrentIndex = Logs.IndexCollection.IndexValues(Index).OrderByDescending(x => x.Count.ToString("000000000")).ToList();
                if (mCurrentIndex != null)
                    DebugProperties.SelectedObject = mCurrentIndex;

                IndexList.VirtualListSize = mCurrentIndex?.Count ?? 0;
                IndexList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        public void DisplayInternalLog()
        {
            //mSelectedIndexType = InternalLogName;
            //mSelectedIndex = "";
            //mCurrentIndex = null;
            //mIndexList.VirtualListSize = 0;
            //mCurrentLines = EventLogCollection.InternalLog;
            //if (mCurrentLines != null)
            //    DebugProperties.SelectedObject = mCurrentLines;
            //LinesList.VirtualListSize = mCurrentLines.Count;
            //LinesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        public void DisplayLines(string IndexType, string IndexValue)
        {
            mCurrentLines = Logs.IndexCollection.Lines(IndexType, IndexValue);
            if (mCurrentLines != null)
            {
                DebugProperties.SelectedObject = mCurrentLines;
                LinesList.VirtualListSize = mCurrentLines.Count;
                LinesList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        public void Filter(string SearchText)
        {
            mCurrentLines = Logs.IndexCollection.Lines(mSelectedIndexType, mSelectedIndex);
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
        ///     ''' Set status msg... would probably be better to have the display subscribe to events
        ///     ''' </summary>
        ///     ''' <param name="Msg"></param>
        ///     ''' <remarks></remarks>
        public void SetStatus(string Msg)
        {
            StatusBar.Text = Msg;
        }

        //private string StripInvalidFileCharacters(string stringWithInvalidChars)
        //{
        //    char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();

        //    var invalidCharsRemoved = stringWithInvalidChars.Where(x => !invalidChars.Contains(x)).ToArray();

        //    return invalidCharsRemoved;
        //}

        /// <summary>
        ///     ''' Save the current list of lines to a text file.
        ///     ''' </summary>
        ///     ''' <param name="FileName"></param>
        ///     ''' <remarks></remarks>
        //public void SaveCurrentLines(string FileName)
        //{
        //    FileInfo ExePath = new FileInfo(Assembly.GetExecutingAssembly().Location);

        //    mCurrentLines.SaveToFile(ExePath.Directory.FullName + @"\" + StripInvalidFileCharacters(mSelectedIndexType + " - " + mSelectedIndex) + ".txt");
        //}

        ///// <summary>
        /////     ''' Save the list of indicies and their counts to a text file.
        /////     ''' </summary>
        /////     ''' <param name="FileName"></param>
        /////     ''' <remarks></remarks>
        //public void SaveCurrentIndicies(string FileName)
        //{
        //    FileInfo ExePath = new FileInfo(Assembly.GetExecutingAssembly().Location);
        //    string MsgIndent = "".PadLeft(8) + Constants.vbTab + "".PadLeft(22) + Constants.vbTab + "".PadLeft(22) + Constants.vbTab;
        //    StringBuilder msg = new StringBuilder();
        //    foreach (Index.IndexDisplayLine idl in mCurrentIndex)
        //        msg.AppendLine(idl.Count.ToString().PadLeft(8) + Constants.vbTab + idl.First.ToString().PadLeft(22) + Constants.vbTab + idl.Last.ToString().PadLeft(22) + Constants.vbTab + idl.IndexValue.Replace(Constants.vbNewLine, Constants.vbNewLine + MsgIndent));

        //    My.Computer.FileSystem.WriteAllText(ExePath.Directory.FullName + @"\" + StripInvalidFileCharacters(mSelectedIndexType) + ".txt", msg.ToString(), false);
        //}

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

            if (timer == null)
                return;

            string SearchText = timer.Tag.ToString() ?? string.Empty;
            Filter(SearchText);
            timer.Stop();
        }

        private void mFileList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (mFileList.SelectedIndices.Count < 1)
                return;

            mCurrentLines = Logs.Logs[mFileList.SelectedIndices[0]].FilteredEvents;
            if (mCurrentLines != null)
                DebugProperties.SelectedObject = Logs.Logs[mFileList.SelectedIndices[0]];
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
                        mCurrentIndex = mCurrentIndex.OrderBy(x => x.IndexValue).ToList();
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
            IndexValue.IndexValue,
            IndexValue.First.ToString()?? string.Empty,
            IndexValue.Last.ToString() ?? string.Empty
            });
        }

        private void mIndexList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (IndexList.SelectedIndices.Count < 1)
                return;

            var IndexValue = mCurrentIndex[mIndexList.SelectedIndices[0]];
            string IndexSelected = IndexValue.IndexValue;

            // only do anything if we are changing the active indextype
            if (IndexSelected != mSelectedIndex)
            {
                mSelectedIndex = IndexSelected;
                // display the new lines
                DisplayLines(mSelectedIndexType, mSelectedIndex);

                DebugProperties.SelectedObject = Logs.IndexCollection[mSelectedIndexType][mSelectedIndex];

                // select the first line
                mLinesList.SelectedIndices.Clear();
                if (mLinesList.VirtualListSize > 0)
                    LinesList.SelectedIndices.Add(0);
            }
        }

        private void mIndexTypeList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (IndexTypeList.SelectedItems.Count < 1)
                return;

            string IndexType = IndexTypeList.SelectedItems[0].SubItems[1].Text;

            // only do anything if we are changing the active indextype
            // If IndexType <> mSelectedIndexType Then
            mSelectedIndexType = IndexType;
            // display the new indicies
            DisplayIndices(mSelectedIndexType);
        }

        private void mLinesList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var Line = mCurrentLines[e.ItemIndex];
            string[] LineInfo;

            // This is a hack not meant to be permanent
            if (mSelectedIndex == "AllFilesCombined")
            {
                FileInfo f = new FileInfo(Line.LogFileName);
                LineInfo = new string[] { Line.Timestamp.ToString() ?? string.Empty, Line.Level, f.Name + " - " + Line.Message };
            }
            else
                LineInfo = new string[] { Line.Timestamp.ToString() ?? string.Empty, Line.Level, Line.Message };
            e.Item = new ListViewItem(LineInfo);
        }

        private void mLinesList_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (mLinesList.SelectedIndices.Count < 1)
                return;

            var Line = mCurrentLines[mLinesList.SelectedIndices[0]];
            DebugProperties.SelectedObject = Line;
            DetailText.Text = Line.Message;
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

        private void mSearchBox_TextChanged(object sender, EventArgs e)
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