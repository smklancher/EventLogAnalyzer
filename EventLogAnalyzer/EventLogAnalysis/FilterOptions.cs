using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventLogAnalysis.Filtering;

namespace EventLogAnalysis
{
    public class FilterOptions
    {
        public FilterOptions(string include, string exclude)
        {
            if (Options.Instance.CsvSearchTerms)
            {
                Includes = include.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                Excludes = exclude.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                FilterSet.Filters.AddRange(Includes.Select(x => TermToFilter(x, FilterAction.Include)));
                FilterSet.Filters.AddRange(Excludes.Select(x => TermToFilter(x, FilterAction.Exclude)));

                DescribeFilters();
            }
            else
            {
                Includes = new();
                Excludes = new();
            }
        }

        public List<Filter> ExcludeFilters => FilterSet.Filters.Where(x => x.Action == FilterAction.Exclude).ToList();
        public List<string> Excludes { get; set; }
        public string ExcludeText { get; set; } = string.Empty;
        public FilterSet FilterSet { get; set; } = new FilterSet();
        public List<Filter> IncludeFilters => FilterSet.Filters.Where(x => x.Action == FilterAction.Include).ToList();

        public List<string> Includes { get; set; }

        public string SearchText { get; set; } = string.Empty;

        public static bool ResultsAreSubset(FilterOptions old, FilterOptions newf)
        {
            //searchText.Length > LastSearchText.Length && searchText.StartsWith(LastSearchText)

            if (!Options.Instance.CsvSearchTerms)
            {
                // simple search:
                // if you just keep typing include search or deleting exclude search, then
                // the results will continue to be a smaller subset of existing results
                return newf.SearchText.StartsWith(old.SearchText, StringComparison.OrdinalIgnoreCase) &&
                    old.ExcludeText.StartsWith(newf.ExcludeText, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                //// can only be a subset if include terms are the same or less
                //var includeTermsSameOrLess = newf.Includes.Count <= old.Includes.Count;

                //// can only be a subset if exclude terms are the same or more
                //var excludeTermsSameOrMore = newf.Excludes.Count >= old.Excludes.Count;

                //return includeTermsSameOrLess && excludeTermsSameOrMore &&
                //    newf.SearchText.StartsWith(old.SearchText, StringComparison.OrdinalIgnoreCase) &&
                //    old.ExcludeText.StartsWith(newf.ExcludeText, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public string DescribeFilters()
        {
            var sb = new StringBuilder();
            foreach (var filter in IncludeFilters)
            {
                sb.AppendLine(filter.ToString());
            }

            foreach (var filter in ExcludeFilters)
            {
                sb.AppendLine(filter.ToString());
            }

            var desc = sb.ToString();

            Trace.WriteLine(desc);

            return desc;
        }

        private static Filter TermToFilter(string term, FilterAction action)
        {
            if (DateTime.TryParse(term, out var date))
            {
                var col = FilterColumn.New(
                    typeof(LogEntry),
                    x => ((LogEntry)x).Timestamp.ToString() ?? string.Empty,
                    "TimeStamp");
                col.DateFunc = x => TimestampOptions.Convert(((LogEntry)x).Timestamp);

                var filter = new Filter()
                {
                    Column = col,
                    Value = term,
                    DateValue = date,
                    Action = action,
                    Relation = new RelationGreaterThan(),
                };

                // for now, date in include is a start date (exclude less than)
                //  and date in exclude is end date (exclude greater than)
                if (action == FilterAction.Include)
                {
                    filter.Relation = new RelationLessThan();

                    //this means it changes to an exclude filter
                    filter.Action = FilterAction.Exclude;
                }
                else
                {
                    filter.Relation = new RelationGreaterThan();
                }

                return filter;
            }
            else
            {
                var filter = new Filter()
                {
                    Column = FilterColumn.New(
                    typeof(LogEntry),
                    x => ((LogEntry)x).Message,
                    "Log Message"),
                    Value = term,
                    Action = action,
                    Relation = new RelationContains(),
                };

                return filter;
            }
        }
    }
}