using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq.Extensions;

namespace EventLogAnalysis.Filtering
{
    public class FilterSet
    {
        public List<Filter> Filters { get; set; } = new List<Filter>();

        public void ReplaceQuickFilters(List<Filter> filters)
        {
            Filters.RemoveAll(x => x.IsQuickFilter);
            Filters.AddRange(filters);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var filter in Filters)
            {
                sb.AppendLine(filter.ToString());
            }

            var desc = sb.ToString();

            Trace.WriteLine(desc);

            return desc;
        }
    }
}