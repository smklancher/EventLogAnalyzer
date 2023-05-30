namespace EventLogAnalysis;

public class InternalLog : LogBase<InternalLogLine>
{
    private static readonly Lazy<InternalLog> Lazy =
                                new Lazy<InternalLog>(() => new InternalLog());

    private InternalLog()
    {
        SourceName = "Internal Log";
        TypeName = "InternalLog";
    }

    public static InternalLog Instance => Lazy.Value;
    public InternalLogLineCollection Entries { get; private set; } = new();
    public override ILogEntryCollection<InternalLogLine> EntryCollection => Entries;
    public List<string> LogList { get; private set; } = new();

    public static void WriteLine(string message)
    {
        Instance.Entries.Add(new InternalLogLine(message));
    }
}

public class InternalLogLine : LogEntry
{
    public InternalLogLine(string msg)
    {
        Message = msg;
        Timestamp = DateTime.Now;
    }
}

//public class StringAsLogEntry : LogEntry
//{
//    public StringAsLogEntry(string msg)
//    {
//        Message = msg;
//    }
//}

//public class StringEntryCollection : LogEntryCollection<StringAsLogEntry>
//{
//    public StringEntryCollection(List<string> list)
//    {
//        this.list = list;
//    }

//    public override IEnumerable<StringAsLogEntry> Entries => list.Select(x => new StringAsLogEntry(x)).ToList();
//    private List<string> list { get; }
//}

public class InternalLogLineCollection : LogEntryCollection<InternalLogLine>
{
}