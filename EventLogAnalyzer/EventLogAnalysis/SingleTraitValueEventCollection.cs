namespace EventLogAnalysis;

public class SingleTraitValueEventCollection : LogEntryCollection<LogEntry>
{
    public string TraitName { get; set; } = string.Empty;
    public string TraitValue { get; set; } = string.Empty;
}