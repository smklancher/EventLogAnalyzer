using Timer = System.Windows.Forms.Timer;

namespace EventLogAnalyzer;

public partial class GenericLogCollectionDisplay
{
    private PropertyGrid mDebugProperties = new();
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

    public TextBox DetailText { get; private set; } = new();

    public FileListView FileList { get; private set; }

    public LinesListView LinesList { get; private set; }
    public LogCollection Logs { get; private set; } = new();

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

    public TraitTypesListView TraitTypesList { get; private set; }
    public TraitValuesListView TraitValuesList { get; private set; }
}