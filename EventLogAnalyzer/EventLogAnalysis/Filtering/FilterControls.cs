using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    public class FilterControls
    {
        public FilterControls(ComboBox columns, ComboBox relations, ComboBox values, ComboBox actions, FilterListView list)
        {
            Columns = columns;
            Relations = relations;
            Values = values;
            Actions = actions;
            List = list;

            Columns.Items.AddRange(KnownFilter.Columns.Keys.ToArray());
            Relations.Items.AddRange(KnownFilter.Relations.Keys.ToArray());
            Actions.Items.AddRange(KnownFilter.Actions.Keys.ToArray());

            Columns.SelectedValueChanged += Columns_SelectedValueChanged;
        }

        public ComboBox Actions { get; private set; }

        public ComboBox Columns { get; private set; }

        public FilterListView List { get; private set; }

        public ComboBox Relations { get; private set; }

        public ComboBox Values { get; private set; }

        public void AddFilter()
        {
            List.Add(Columns.Text, Relations.Text, Values.Text, Actions.Text);
        }

        public void PopSelectedFilterForEdit()
        {
            var filter = List.RemoveSelected();
            if (filter != null)
            {
                Columns.Text = filter.Column.DisplayName;
                Relations.Text = filter.Relation.DisplayName;
                Values.Text = filter.Value;
                Actions.Text = filter.Action.ToString();
            }
        }

        public FilterColumn SelectedColumnFilter()
        {
            return KnownFilter.Columns[Columns.Text];
        }

        private void Columns_SelectedValueChanged(object? sender, EventArgs e)
        {
            var f = SelectedColumnFilter();

            if (f.SuggestedValuesFunc != null)
            {
                Values.Items.Clear();
                var values = f.SuggestedValuesFunc.Invoke();
                Values.Items.AddRange(values.ToArray());
            }
        }
    }
}