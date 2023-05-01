using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    public class FilterControls
    {
        public FilterControls(ComboBox columns, ComboBox relations, ComboBox values, ComboBox actions)
        {
            Columns = columns;
            Relations = relations;
            Values = values;
            Actions = actions;

            Columns.Items.AddRange(KnownFilter.Columns.Keys.ToArray<string>());
            Relations.Items.AddRange(KnownFilter.Relations.Keys.ToArray<string>());
            Actions.Items.AddRange(KnownFilter.Actions.Keys.ToArray<string>());
        }

        public ComboBox Actions { get; set; }
        public ComboBox Columns { get; set; }

        public ComboBox Relations { get; set; }
        public ComboBox Values { get; set; }
    }
}