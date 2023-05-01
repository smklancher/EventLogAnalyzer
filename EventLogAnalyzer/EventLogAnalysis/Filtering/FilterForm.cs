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
            controls = new FilterControls(ColumnDropdown, RelationDropdown, ValueDropdown, ActionDropdown);
            list = new FilterListView(FilterList);
        }

        private void FilterForm_Load(object sender, EventArgs e)
        {
        }
    }
}