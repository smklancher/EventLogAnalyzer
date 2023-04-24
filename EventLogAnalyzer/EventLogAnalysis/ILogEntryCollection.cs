// needs to be an interface to take advangage of generic covariance (not supported for classes)
// ILogEntryCollection<out T> where T : LogEntry
// then a function could return an EventCollction (ILogEntryCollection<ELRecord>) as ILogEntryCollection<LogEntry> to use generically in UI, etc.
// And Entries becomes IEnumerable for covariance as well

namespace EventLogAnalysis;

public interface ILogEntryCollection<out T> where T : LogEntry
{
    public ILogEntryCollection<LogEntry> AsLogEntries { get; }

    public IEnumerable<T> Entries { get; }

    /// <summary>
    /// Returns the date of the first event in the collection... currently this causes a sort (costly))
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public DateTime FirstEvent { get; }

    /// <summary>
    /// Returns the date of the last event in the collection... currently this causes a sort (costly))
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public DateTime LastEvent { get; }

    public ILogEntryCollection<T> FilteredCopy(FilterOptions filterOptions);

    public ILogEntryCollection<T> FilteredCopy(string FilterText);
}