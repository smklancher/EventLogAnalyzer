using System.Diagnostics;

namespace EventLogAnalysis;

public class LogEntry : IComparable<LogEntry>, IEquatable<LogEntry>
{
    //TODO: review use of protected properties

    public Color? Color { get; set; }
    public string Level { get; protected set; } = string.Empty;
    public string Message { get; protected set; } = string.Empty;

    public long MessageCharacterCount => Message.Length;
    public string ShortMessage { get; set; } = string.Empty;
    public virtual DateTime? Timestamp { get; protected set; }
    public virtual string UniqueId { get; protected set; } = new Guid().ToString();

    public int CompareTo(LogEntry? other)
    {
        return (Timestamp ?? DateTime.MaxValue).CompareTo(other?.Timestamp ?? DateTime.MaxValue);
    }

    public bool Equals(LogEntry? other)
    {
        if (other is null) { return false; }
        if (object.ReferenceEquals(this, other)) { return true; }

        return UniqueId == other.UniqueId;
    }

    public override int GetHashCode()
    {
        return UniqueId.GetHashCode();
    }
}