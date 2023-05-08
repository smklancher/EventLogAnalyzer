using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis.Filtering
{
    public class FilterColumn
    {
        //couldn't figure out how to use generics for the type without co/contra variance issues
        private Func<object, string> valueFunc;

        private FilterColumn(Type type, Func<object, string> valueFunc, string displayName)
        {
            this.valueFunc = valueFunc;
            Type = type;
            DisplayName = displayName;
        }

        public Func<object, DateTime?>? DateFunc { get; set; }

        public string DisplayName { get; init; }

        public bool IsDate => DateFunc != null;

        public Func<IEnumerable<string>>? SuggestedValuesFunc { get; set; }
        public Type Type { get; init; }

        public static FilterColumn New(Type type, Func<object, string> valueFunc, string displayName)
        {
            var fc = new FilterColumn(type, valueFunc, displayName);
            KnownFilter.Columns.TryAdd(fc.DisplayName, fc);
            return fc;
        }

        public static void test()
        {
            var x = new FilterColumn(typeof(LogEntry), x => ((LogEntry)x).Message, "Log Message");
        }

        public DateTime? DateValue(object obj) => DateFunc?.Invoke(obj);

        public string ObjectValue(object obj) => valueFunc(obj);

        private void Notes()
        {
            // needs to be an interface to take advangage of generic covariance (not supported for classes)
            // ILogEntryCollection<out T> where T : LogEntry
            // then a function could return an EventCollction (ILogEntryCollection<ELRecord>) as ILogEntryCollection<LogEntry> to use generically in UI, etc.
            // And Entries becomes IEnumerable for covariance as well
            var x = nameof(ILogEntryCollection<LogEntry>);

            // here I want FilterColumn to have an input type like FilterColumn<LogEntry>
            // and a method that returns a string from a property on that type
        }
    }
}