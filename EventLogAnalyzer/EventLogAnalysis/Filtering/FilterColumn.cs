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

        public FilterColumn(Type type, Func<object, string> valueFunc, string displayName)
        {
            this.valueFunc = valueFunc;
            Type = type;
            DisplayName = displayName;
        }

        public string DisplayName { get; init; }
        public Type Type { get; init; }

        public static void test()
        {
            var x = new FilterColumn(typeof(LogEntry), x => ((LogEntry)x).Message, "Log Message");
        }

        public void Notes()
        {
            // needs to be an interface to take advangage of generic covariance (not supported for classes)
            // ILogEntryCollection<out T> where T : LogEntry
            // then a function could return an EventCollction (ILogEntryCollection<ELRecord>) as ILogEntryCollection<LogEntry> to use generically in UI, etc.
            // And Entries becomes IEnumerable for covariance as well
            var x = nameof(ILogEntryCollection<LogEntry>);

            // here I want FilterColumn to have an input type like FilterColumn<LogEntry>
            // and a method that returns a string from a property on that type
        }

        public string ObjectValue(object obj) => valueFunc(obj);
    }
}