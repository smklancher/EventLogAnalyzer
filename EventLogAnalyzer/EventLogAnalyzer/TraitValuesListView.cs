using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventLogAnalysis;

namespace EventLogAnalyzer
{
    public class TraitValuesListView
    {
        public TraitValuesListView(ListView lv, LinesListView llv, PropertyGrid propertyGrid)
        {
            LinesList = llv;
            list = lv;
            list.BeginUpdate();
            list.View = View.Details;
            list.FullRowSelect = true;
            list.Columns.Add("#");
            if (!Options.Instance.TraitDatesBeforeTraitValue) { list.Columns.Add("Trait Value"); }
            list.Columns.Add("First");
            list.Columns.Add("Last");
            if (Options.Instance.TraitDatesBeforeTraitValue) { list.Columns.Add("Trait Value"); }
            list.VirtualMode = true;
            list.VirtualListSize = 0;

            list.RetrieveVirtualItem += List_RetrieveVirtualItem;
            list.SelectedIndexChanged += List_SelectedIndexChanged;
            list.ColumnClick += List_ColumnClick;

            DebugProperties = propertyGrid;

            list.EndUpdate();
        }

        /// <summary>
        /// is this actually needed or just use UI selection?
        /// </summary>
        public string ActiveTraitValue { get; private set; } = string.Empty;

        public string CurrentTraitType => CurrentTraitValues.Name;
        public TraitValuesCollection CurrentTraitValues { get; private set; } = new(string.Empty);

        public List<TraitValuesCollection.TraitValueSummaryLine> CurrentTraitValueSummaries { get; private set; } = new();
        public PropertyGrid DebugProperties { get; }
        public bool IsDisplayingInternalLog { get; private set; } = false;

        public LinesListView LinesList { get; }
        private ListView list { get; }

        public void DisplayInternalLog()
        {
            IsDisplayingInternalLog = true;
            ActiveTraitValue = "";
            list.BeginUpdate();
            list.SelectedIndices.Clear();
            list.VirtualListSize = 0;
            list.Invalidate();
            list.EndUpdate();

            LinesList.DisplayInternalLog();
        }

        public string SelectedTraitValue()
        {
            if (list.SelectedIndices.Count < 1) { return string.Empty; }
            return CurrentTraitValueSummaries[list.SelectedIndices[0]].TraitValue;
        }

        public void SelectFirstLine()
        {
            list.BeginUpdate();
            list.SelectedIndices.Clear();
            if (list.VirtualListSize > 0)
            {
                list.SelectedIndices.Add(0);
            }
            list.EndUpdate();
        }

        public void UpdateTraitValuesSource(TraitValuesCollection newsource)
        {
            list.BeginUpdate();
            IsDisplayingInternalLog = false;
            CurrentTraitValues = newsource;
            list.VirtualListSize = CurrentTraitValues.Count;

            CurrentTraitValueSummaries = CurrentTraitValues.ValuesCounts().OrderByDescending(x => x.Count.ToString("000000000")).ToList();

            SelectFirstLine();

            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            list.EndUpdate();

            InternalDisplayLines();
        }

        private void InternalDisplayLines() => LinesList.UpdateLineSource(CurrentTraitValues.LinesFromTraitValue(SelectedTraitValue()));

        private void List_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
            // should change to a kind of strongly typed column concept (maybe via listview extension methods)

            var mCurrentIndex = CurrentTraitValueSummaries;

            list.BeginUpdate();

            switch (e.Column)
            {
            case 0:
                {
                    CurrentTraitValueSummaries = CurrentTraitValueSummaries.OrderByDescending(x => x.Count.ToString("000000000")).ToList();
                    break;
                }

            case 1:
                {
                    if (Options.Instance.TraitDatesBeforeTraitValue)
                    {
                        CurrentTraitValueSummaries = CurrentTraitValueSummaries.OrderBy(x => x.First).ToList();
                    }
                    else
                    {
                        CurrentTraitValueSummaries = CurrentTraitValueSummaries.OrderBy(x => x.TraitValue).ToList();
                    }
                    break;
                }

            case 2:
                {
                    if (Options.Instance.TraitDatesBeforeTraitValue)
                    {
                        CurrentTraitValueSummaries = CurrentTraitValueSummaries.OrderBy(x => x.Last).ToList();
                    }
                    else
                    {
                        CurrentTraitValueSummaries = CurrentTraitValueSummaries.OrderBy(x => x.First).ToList();
                    }
                    break;
                }

            case 3:
                {
                    if (Options.Instance.TraitDatesBeforeTraitValue)
                    {
                        CurrentTraitValueSummaries = CurrentTraitValueSummaries.OrderBy(x => x.TraitValue).ToList();
                    }
                    else
                    {
                        CurrentTraitValueSummaries = CurrentTraitValueSummaries.OrderBy(x => x.Last).ToList();
                    }
                    break;
                }
            }

            list.Invalidate();
            list.EndUpdate();
        }

        private void List_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var summary = CurrentTraitValueSummaries[e.ItemIndex];
            e.Item = new ListViewItem(summary.AsArray(Options.Instance.TraitDatesBeforeTraitValue));
        }

        private void List_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // only do anything if we are changing the active indextype
            if (SelectedTraitValue() != ActiveTraitValue)
            {
                //unset search text
                //SearchBox.Text = string.Empty;

                ActiveTraitValue = SelectedTraitValue();
                // display the new lines
                InternalDisplayLines();
            }
        }
    }
}