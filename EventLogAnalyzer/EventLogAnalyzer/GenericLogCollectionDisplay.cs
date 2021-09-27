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
        //public List<string> InternalLog = new();
        private const string InternalLogName = "InternalLog";

        private string LastSearchText = "";

        public GenericLogCollectionDisplay(ListView lstLines, ListView lstIndex, ListView lstIndexType, ListView lstFiles, TextBox txtDetail, PropertyGrid propertyGrid)
        {
            ;
            DetailText = txtDetail;
            DebugProperties = propertyGrid;

            LinesList = new LinesListView(lstLines, DetailText, DebugProperties);
            Log.Logger = LinesList.InternalLog.AsSeriLogger();
            TraitValuesList = new TraitValuesListView(lstIndex, LinesList, DebugProperties);
            TraitTypesList = new TraitTypesListView(lstIndexType, TraitValuesList);
            FileList = new FileListView(lstFiles, LinesList);
        }

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

        public void DisplayInternalLog()
        {
            TraitValuesList.DisplayInternalLog();
            LinesList.DisplayInternalLog();
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
            if (TraitTypesList.SelectedTraitType() == InternalLogName)
            {
                DisplayInternalLog();
            }
            else
            {
                //var summaries = Logs.TraitTypes.TraitValueSummaries(TraitType).OrderByDescending(x => x.Count.ToString("000000000")).ToList();
                //TraitValuesList.UpdateTraitValueSummarySource(summaries);

                var values = Logs.TraitTypes.TraitValues(TraitTypesList.SelectedTraitType());
                TraitValuesList.UpdateTraitValuesSource(values);
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
                var newlines = LinesFromFileOrTraitValue();
                newlines = newlines.FilteredCopy(searchText);
                LinesList.UpdateLineSource(newlines);
            }
            LastSearchText = searchText;
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
            var timer = sender as Timer;

            if (timer is not null)
            {
                string SearchText = timer.Tag.ToString() ?? string.Empty;
                Filter(SearchText);
                timer.Stop();
            }
        }

        private EventCollection LinesFromFileOrTraitValue() =>
            IsDisplayingFullFile ? FileList.SelectedLogEvents : Logs.TraitTypes.Lines(TraitTypesList.SelectedTraitType(), TraitValuesList.ActiveTraitValue);

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