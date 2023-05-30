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
        public List<Filter> ExcludeFilters => FilterSet.Filters.Where(x => x.Action == FilterAction.Exclude).ToList();

        public string ExcludeText { get; set; } = string.Empty;

        public FilterSet FilterSet { get; set; } = new FilterSet();

        public List<Filter> HighlightFilters => FilterSet.Filters.Where(x => x.Action == FilterAction.Hightlight).ToList();

        public List<Filter> IncludeFilters => FilterSet.Filters.Where(x => x.Action == FilterAction.Include).ToList();

        public string SearchText { get; set; } = string.Empty;

        public static void AddTraitsAsTypeColumns(LogCollection logs)
        {
            foreach (var traitName in logs.TraitTypes.Keys)
            {
                FilterColumn.New(
                typeof(LogEntry),
                x => ((LogEntry)x).GetTraitByName(traitName),
                traitName);
            }
        }

        /// <summary>
        /// Add filter columns to known filters... probably these should be on the types they apply to, to allow for extensible plugins
        /// </summary>
        public static void InitializeFilterColumns()
        {
            var col = FilterColumn.New(
                    typeof(LogEntry),
                    x => ((LogEntry)x).Timestamp.ToString() ?? string.Empty,
                    "TimeStamp");
            col.DateFunc = x => TimestampOptions.Convert(((LogEntry)x).Timestamp);

            FilterColumn.New(
                typeof(LogEntry),
                x => ((LogEntry)x).Message,
                "Log Message");

            col = FilterColumn.New(
                typeof(LogEntry),
                x => ((LogEntry)x).Level,
                "Log Level");

            col.SuggestedValuesFunc = () => Enum.GetNames(typeof(TraceEventType));
        }

        /// <summary>
        /// Create filters from csv inputs
        /// </summary>
        /// <param name="include">CSV: Each term creates a contains include filter.  Dates become a start date filter (exclude less than).</param>
        /// <param name="exclude">CSV: Each term creates a contains exclude filter.  Dates become a end date filter (exclude greater than).</param>
        /// <returns></returns>
        public static List<Filter> QuickFilters(string include, string exclude)
        {
            var list = new List<Filter>();

            var incList = include.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var excList = exclude.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            list.AddRange(incList.Select(x => TermToQuickFilter(x, FilterAction.Include)));
            list.AddRange(excList.Select(x => TermToQuickFilter(x, FilterAction.Exclude)));

            return list;
        }

        public static bool ResultsAreSubset(FilterOptions old, FilterOptions newf)
        {
            return false;

            // simple search:
            // if you just keep typing include search or deleting exclude search, then
            // the results will continue to be a smaller subset of existing results
            //return newf.SearchText.StartsWith(old.SearchText, StringComparison.OrdinalIgnoreCase) &&
            //    old.ExcludeText.StartsWith(newf.ExcludeText, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Probably move implementation from LogEntryCollection to FilterOptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unfiltered"></param>
        /// <returns></returns>
        public ILogEntryCollection<T> FilteredCopy<T>(LogEntryCollection<T> unfiltered) where T : LogEntry
        {
            return unfiltered.FilteredCopy(this);
        }

        private static Filter TermToQuickFilter(string term, FilterAction action)
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

                filter.IsQuickFilter = true;
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

                filter.IsQuickFilter = true;
                return filter;
            }
        }
    }
}