using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class InternalLog : LogBase<StringAsLogEntry>
    {
        public InternalLog()
        {
            SourceName = "Internal Log";
            TypeName = "InternalLog";
        }

        public override StringEntryCollection EntryCollection => new StringEntryCollection(LogList);
        public List<string> LogList { get; private set; } = new();
    }

    public class StringAsLogEntry : LogEntry
    {
        public StringAsLogEntry(string msg)
        {
            Message = msg;
        }
    }

    public class StringEntryCollection : ILogEntryCollection<StringAsLogEntry>
    {
        public StringEntryCollection(List<string> list)
        {
            this.list = list;
        }

        public IEnumerable<StringAsLogEntry> Entries => list.Select(x => new StringAsLogEntry(x)).ToList();
        private List<string> list { get; }
    }
}