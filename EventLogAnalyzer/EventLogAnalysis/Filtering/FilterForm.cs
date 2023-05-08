using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventLogAnalysis.Filtering
{
    public partial class FilterForm : Form
    {
        private FilterControls controls;
        private FilterListView list;

        public FilterForm()
        {
            InitializeComponent();
            KnownFilter.Init();
            list = new FilterListView(FilterList);
            controls = new FilterControls(ColumnDropdown, RelationDropdown, ValueDropdown, ActionDropdown, list);
        }

        public void LoadFilters(FilterSet filterSet)
        {
            list.LoadFilters(filterSet);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            controls.AddFilter();
        }

        private void FilterList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            controls.PopSelectedFilterForEdit();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            controls.PopSelectedFilterForEdit();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            var errors = new Filter()
            {
                Column = KnownFilter.Columns["Log Level"],
                Value = "Error",
                Action = FilterAction.Hightlight,
                Relation = new RelationContains(),
                HighlightColor = Color.LightSalmon,
            };

            var warnings = new Filter()
            {
                Column = KnownFilter.Columns["Log Level"],
                Value = "Warning",
                Action = FilterAction.Hightlight,
                Relation = new RelationContains(),
                HighlightColor = Color.PaleGoldenrod,
            };

            list.FilterSet.Filters.Clear();
            list.Add(errors);
            list.Add(warnings);
        }
    }
}