using System;
using System.Collections.Generic;
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
            list.Columns.Add("Column");
            list.Columns.Add("Relation");
            list.Columns.Add("Value");
            list.Columns.Add("Action");

            list.VirtualMode = true;
            list.VirtualListSize = 0;

            //list.SelectedIndexChanged += List_SelectedIndexChanged;
            //list.RetrieveVirtualItem += List_RetrieveVirtualItem;

            list.EndUpdate();
        }
    }
}