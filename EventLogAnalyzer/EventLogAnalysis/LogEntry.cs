using System.Diagnostics;
using System.Linq;

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

    public virtual string UniqueId { get; protected set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// May want to rethink how traits are stored...
    /// originally stored on the log, but need on the line for filtering and using the existing types for now
    /// </summary>
    private TraitTypeCollection Traits { get; } = new TraitTypeCollection();

    public void AddTrait(string traitName, string traitValue, TraitTypeCollection? logTraits = null)
    {
        if (logTraits != null)
        {
            logTraits.AddLine(traitName, traitValue, this);
        }

        Traits.AddLine(traitName, traitValue, this);
    }

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

    public string GetTraitByName(string name)
    {
        // using existing types for now even though it's an odd fit
        if (Traits.TryGetValue(name, out var traitValuesCollection))
        {
            // any trait that exists should only have the one value for this line
            if (traitValuesCollection.Count > 0)
            {
                return traitValuesCollection.Keys.ToList()[0];
            }
        }

        return string.Empty;
    }
}