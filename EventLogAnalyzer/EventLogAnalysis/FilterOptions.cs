using System;
using System.Collections.Generic;
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

                IncludeFilters = Includes.Select(x => TermToFilter(x, FilterAction.Include)).ToList();
                ExcludeFilters = Excludes.Select(x => TermToFilter(x, FilterAction.Exclude)).ToList();
            }
            else
            {
                Includes = new();
                Excludes = new();
                IncludeFilters = new();
                ExcludeFilters = new();
            }
        }

        public List<Filter> ExcludeFilters { get; set; }

        public List<string> Excludes { get; set; }

        public string ExcludeText { get; set; } = string.Empty;

        public List<Filter> IncludeFilters { get; set; }

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

        private static Filter TermToFilter(string term, FilterAction action)
        {
            var filter = new Filter()
            {
                Column = new FilterColumn(
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