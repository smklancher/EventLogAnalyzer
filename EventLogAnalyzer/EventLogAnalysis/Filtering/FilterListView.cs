using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    public class FilterListView
    {
        private ListView list;

        public FilterListView(ListView lv)
        {
            list = lv;
            list.BeginUpdate();
            list.View = View.Details;
            list.FullRowSelect = true;
            list.CheckBoxes = true;
            list.Columns.Add("Column");
            list.Columns.Add("Relation");
            list.Columns.Add("Value");
            list.Columns.Add("Action");

            list.VirtualMode = true;
            list.VirtualListSize = 0;

            //list.SelectedIndexChanged += List_SelectedIndexChanged;
            list.RetrieveVirtualItem += List_RetrieveVirtualItem;
            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            list.EndUpdate();
        }

        public FilterSet FilterSet { get; private set; } = new FilterSet();

        public void Add(Filter filter)
        {
            FilterSet.Filters.Add(filter);
            Refresh();
        }

        public void Add(string ColumnName, string RelationName, string Value, string ActionName)
        {
            bool validFilter = true;
            validFilter &= KnownFilter.Columns.TryGetValue(ColumnName, out var column);
            validFilter &= KnownFilter.Relations.TryGetValue(RelationName, out var relation);
            validFilter &= KnownFilter.Actions.TryGetValue(ActionName, out var action);

            var f = new Filter()
            {
                Column = column!,
                Relation = relation!,
                Value = Value,
                Action = action,
            };

            if (validFilter)
            {
                Add(f);
            }
            else
            {
                Trace.WriteLine($"Attempt to add invalid filter: {f}");
            }
        }

        public void LoadFilters(FilterSet filterSet)
        {
            FilterSet = filterSet;
            Refresh();
        }

        public void Refresh()
        {
            list.VirtualListSize = FilterSet.Filters.Count;
            if (FilterSet.Filters.Count > 0)
            {
                list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        public Filter? RemoveSelected()
        {
            var f = SelectedFilter();
            if (f == null) { return null; }

            FilterSet.Filters.Remove(f);
            Refresh();
            return f;
        }

        public Filter? SelectedFilter()
        {
            if (list.SelectedIndices.Count < 1) { return null; }
            var index = list.SelectedIndices[0];
            if (index < 0 || index > FilterSet.Filters.Count - 1) { return null; }

            return FilterSet.Filters[index];
        }

        private void List_RetrieveVirtualItem(object? sender, RetrieveVirtualItemEventArgs e)
        {
            string[] LineInfo;
            int index = e.ItemIndex;

            var f = FilterSet.Filters[index];

            LineInfo = new string[] { f.Column.DisplayName, f.Relation.DisplayName, f.Value, f.Action.ToString() };

            e.Item = new ListViewItem(LineInfo);
        }
    }
}