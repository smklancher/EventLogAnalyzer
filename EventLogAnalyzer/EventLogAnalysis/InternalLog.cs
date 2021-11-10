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

    public class StringEntryCollection : LogEntryCollection<StringAsLogEntry>
    {
        public StringEntryCollection(List<string> list)
        {
            this.list = list;
        }

        public override IEnumerable<StringAsLogEntry> Entries => list.Select(x => new StringAsLogEntry(x)).ToList();
        private List<string> list { get; }
    }
}