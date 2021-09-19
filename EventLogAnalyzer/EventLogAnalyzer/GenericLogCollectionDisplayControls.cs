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
        private ListView mIndexTypeList = new();
        private ToolStripProgressBar mProgressBar = new();
        private TextBox mSearchBox = new();
        private ToolStripStatusLabel mStatusBar = new();
        private Timer? mTypingTimer;

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
                value.Columns.Add("Trait Type");
                mIndexTypeList = value;
                IndexTypeList.Items.Add(new ListViewItem(new[] { "N/A", InternalLogName }));

                mIndexTypeList.SelectedIndexChanged += mIndexTypeList_SelectedIndexChanged;
            }
            get
            {
                return mIndexTypeList;
            }
        }

        public LinesListView LinesList { get; set; }

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
                    //mLogs.LogsFinishedLoading -= mLogs_LogsFinishedLoading;
                }

                mLogs = value;

                // mLogs.LogsFinishedLoading += mLogs_LogsFinishedLoading;
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

        public ToolStripStatusLabel StatusBar
        {
            get => mStatusBar;

            set
            {
                mStatusBar = value;
                mStatusBar.Alignment = ToolStripItemAlignment.Left;
                mStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                mStatusBar.Spring = true;
            }
        }

        public TraitValuesListView TraitValuesList { get; set; }
    }
}