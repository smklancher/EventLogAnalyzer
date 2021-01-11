using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventLogAnalysis;

namespace EventLogAnalyzer
{
    public partial class GenericLogCollectionDisplay
    {
        private PropertyGrid mDebugProperties = new();
        private ListView mFileList = new();
        private ListView mIndexList = new();
        private ListView mIndexTypeList = new();
        private ListView mLinesList = new();
        private ToolStripProgressBar mProgressBar = new();
        private TextBox mSearchBox = new();
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
                IndexTypeList.Items.Add(new ListViewItem(new[] { "N/A", InternalLogName }));

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
                {
                    mLogs = new EventLogCollection();
                }

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

        public TextBox SearchBox
        {
            get => mSearchBox;
            set
            {
                if (mSearchBox is not null)
                {
                    mSearchBox.TextChanged -= mSearchBox_TextChanged;
                }

                mSearchBox = value;
                mSearchBox.TextChanged += mSearchBox_TextChanged;
            }
        }

        public ToolStripStatusLabel StatusBar { set; get; } = new();
    }
}