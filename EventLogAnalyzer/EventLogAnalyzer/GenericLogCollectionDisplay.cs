namespace EventLogAnalyzer;

public partial class GenericLogCollectionDisplay
{
    private string LastExcludeText = "";
    private FilterOptions LastFilterOptions = new FilterOptions(string.Empty, string.Empty);
    private string LastSearchText = "";

    public GenericLogCollectionDisplay(ListView lstLines, ListView lstIndex, ListView lstIndexType, ListView lstFiles, TextBox txtDetail, PropertyGrid propertyGrid, ToolStripStatusLabel statusLabel, ToolStripProgressBar progressBar)
    {
        DetailText = txtDetail;
        DebugProperties = propertyGrid;

        LinesList = new LinesListView(lstLines, DetailText, DebugProperties);
        //Log.Logger = LinesList.InternalLog.AsSeriLogger();
        TraitValuesList = new TraitValuesListView(lstIndex, LinesList, DebugProperties);
        TraitTypesList = new TraitTypesListView(lstIndexType, TraitValuesList);
        FileList = new FileListView(lstFiles, LinesList);

        StatusController = new StatusController(statusLabel, progressBar, FileList);
    }

    public StatusController StatusController { get; private set; }

    public TimestampOptions TimestampConversion { get; } = new TimestampOptions();

    private bool IsDisplayingFullFile => string.IsNullOrEmpty(TraitValuesList.ActiveTraitValue) && string.IsNullOrEmpty(TraitTypesList.SelectedTraitType()) && FileList.IsLogSelected;

    public void DisplayFiles()
    {
        FileList.UpdateLogFilesSource(Logs);
    }

    public void DisplayIndexTypes()
    {
        TraitTypesList.UpdateTraitTypesSource(Logs.TraitTypes);
    }

    public void DisplayLines()
    {
        var newlines = LinesFromFileOrTraitValue();

        if (newlines != null)
        {
            LinesList.UpdateLineSource(newlines);
        }
    }

    public void DisplayTraitValues()
    {
        var values = Logs.TraitTypes.TraitValues(TraitTypesList.SelectedTraitType());
        TraitValuesList.UpdateTraitValuesSource(values);
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
            var newlines = LinesFromFileOrTraitValue();
            newlines = newlines.FilteredCopy(searchText);
            LinesList.UpdateLineSource(newlines);
        }
        LastSearchText = searchText;
    }

    public void Filter(FilterOptions filterOptions)
    {
        // if we can logically be sure that the filter should produce only a subset of the previous filter...
        if (FilterOptions.ResultsAreSubset(LastFilterOptions, filterOptions))
        {
            // then it is better performance to filter what is already displayed...
            LinesList.UpdateLineSource(LinesList.CurrentLines.FilteredCopy(filterOptions));
        }
        else
        {
            //...otherwise need to get content from the source
            var newlines = LinesFromFileOrTraitValue();
            newlines = newlines.FilteredCopy(filterOptions);
            LinesList.UpdateLineSource(newlines);
        }

        LastFilterOptions = filterOptions;
    }

    public void Refresh()
    {
        DisplayFiles();
        DisplayIndexTypes();
        DisplayTraitValues();
        DisplayLines();
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
        var timer = sender as System.Windows.Forms.Timer;

        if (timer is not null)
        {
            //string SearchText = timer.Tag.ToString() ?? string.Empty;
            //Filter(SearchText);
            var filter = new FilterOptions(SearchBox.Text, ExcludeBox.Text);
            Filter(filter);
            timer.Stop();
        }
    }

    private ILogEntryCollection<LogEntry> LinesFromFileOrTraitValue() =>
        IsDisplayingFullFile ? FileList.SelectedLogEvents : Logs.TraitTypes.Lines(TraitTypesList.SelectedTraitType(), TraitValuesList.ActiveTraitValue);

    private void mExcludehBox_TextChanged(object? sender, EventArgs e)
    {
        if (mTypingTimer == null)
        {
            mTypingTimer = new System.Windows.Forms.Timer();
            mTypingTimer.Interval = 300;
            mTypingTimer.Tick += this.handleTypingTimerTimeout;
        }

        mTypingTimer.Stop();
        mTypingTimer.Tag = (sender as TextBox)?.Text ?? string.Empty;
        mTypingTimer.Start();
    }

    private void mLogs_LogsFinishedLoading(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
        Refresh();
        StopProgressBar();

        //if (Logs.PreviousLinesLoaded > 0)
        //    SetStatus("Lines loaded: " + Logs.LinesLoaded - Logs.PreviousLinesLoaded + " (" + Logs.LinesLoaded + " total)");
        //else
        //SetStatus($"Total events loaded: {Logs.TotalEventCount}.  Current filter events: {Logs.FilteredEventCount}");
    }

    private void mSearchBox_TextChanged(object? sender, EventArgs e)
    {
        if (mTypingTimer == null) //mExcludehBox_TextChanged
        {
            mTypingTimer = new System.Windows.Forms.Timer();
            mTypingTimer.Interval = 300;
            mTypingTimer.Tick += this.handleTypingTimerTimeout;
        }

        mTypingTimer.Stop();
        mTypingTimer.Tag = (sender as TextBox)?.Text ?? string.Empty;
        mTypingTimer.Start();
    }
}