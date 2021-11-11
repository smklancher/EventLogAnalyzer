namespace EventLogAnalysis;

public interface ILogCollection<out T> where T : LogBase<LogEntry>
{
}