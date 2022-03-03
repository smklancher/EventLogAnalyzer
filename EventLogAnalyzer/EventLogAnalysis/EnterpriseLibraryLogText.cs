using System.Text.RegularExpressions;

namespace EventLogAnalysis;

public class EnterpriseLibraryLogText
{
    // language=regex
    private const string exPropRegex = @"^(?<key>.+?) - (?<value>.+?)$";

    // language=regex
    private const string msgregex = "Timestamp:(?<timestamp>.+)Message:(?<message>.+)Category:(?<category>.+)Priority:(?<priority>.+)EventId:(?<eventid>.+)" +
        "Severity:(?<severity>.+)Title:(?<title>.+)Machine:(?<machine>.+)App Domain:(?<appdomain>.+)ProcessId:(?<processid>.+)Process Name:(?<processname>.+)Thread Name:(?<threadname>.+)" +
        "Win32 ThreadId:(?<threadid>.+)Extended Properties:(?<extentedproperties>.+)";

    // language=regex
    private const string stackTraceRegex = @"((?=^|:)\s*\bat(?<line>.+?)\n)+";

    // language=regex
    private const string submsgregex = @"\WMessage\s*:(?<msg>.+?)Source\s*:";

    // parse messages written with this formatter:
    // <add type="Microsoft.Practices.EnterpriseLibrary.Logging.Formatters.TextFormatter, Microsoft.Practices.EnterpriseLibrary.Logging, Version=5.0.505.0, Culture=neutral,
    // PublicKeyToken=31bf3856ad364e35" template="Timestamp: {timestamp}{newline} Message: {message}{newline} Category: {category}{newline} Priority: {priority}{newline}
    // EventId: {eventid}{newline} Severity: {severity}{newline} Title:{title}{newline} Machine: {localMachine}{newline} App Domain: {localAppDomain}{newline}
    // ProcessId: {localProcessId}{newline} Process Name: {localProcessName}{newline} Thread Name: {threadName}{newline} Win32 ThreadId:{win32ThreadId}{newline}
    // Extended Properties: {dictionary({key} - {value}{newline})}" name="Text Formatter" />

    public EnterpriseLibraryLogText(string eventText)
    {
        EventText = eventText;
        ParseMessage();
    }

    //might not care about this since already have actual event timestamp.  This would be in the original timezone for what it's worth.
    //public DateTime Timestamp { get; private set; } = DateTime.MinValue;

    public string EventText { get; }
    public List<string> ExceptionMessages { get; } = new();
    public Dictionary<string, string> ExtendedProperties { get; private set; } = new Dictionary<string, string>();
    public string InnerException => ExceptionMessages.LastOrDefault() ?? string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string OuterException => ExceptionMessages.FirstOrDefault() ?? string.Empty;
    public string ProcessId { get; private set; } = string.Empty;
    public string ProcessName { get; private set; } = string.Empty;

    public string RequestUrl { get; private set; } = string.Empty;
    public string StackTrace { get; private set; } = string.Empty;
    public string StackTraceBottomFrame { get; private set; } = string.Empty;
    public string StackTraceTopFrame { get; private set; } = string.Empty;

    public void ParseMessage()
    {
        // outer message
        var ContentMatch = Regex.Match(EventText, msgregex, RegexOptions.Singleline);
        if (ContentMatch.Success)
        {
            Message = ContentMatch.Groups["message"].Value.Trim();
            ProcessName = ContentMatch.Groups["processname"].Value.Trim();
            ProcessId = ContentMatch.Groups["processid"].Value.Trim();

            ParseExtendedProperties(ContentMatch.Groups["extentedproperties"].Value.Trim());

            //each message value of exception and inner exceptions
            // could certainly expand to get other properties
            ContentMatch = Regex.Match(Message, submsgregex, RegexOptions.Singleline);
            while (ContentMatch.Success)
            {
                var msg = ContentMatch.Groups["msg"].Value.Trim();
                ExceptionMessages.Add(msg);
                ContentMatch = ContentMatch.NextMatch();
            }

            ContentMatch = Regex.Match(Message, stackTraceRegex, RegexOptions.Multiline);
            if (ContentMatch.Success)
            {
#if NETFRAMEWORK
                // in .NET Framework CaptureCollection, GroupCollection, and MatchCollection  only implement the non-generic interfaces.
                // https://github.com/dotnet/runtime/issues/13933
                var captures = ContentMatch.Groups["line"].Captures.Cast<Capture>();
#else
                var captures = ContentMatch.Groups["line"].Captures;
#endif

                StackTraceTopFrame = captures.FirstOrDefault()?.Value.Trim() ?? string.Empty;
                StackTraceBottomFrame = captures.LastOrDefault()?.Value.Trim() ?? string.Empty;

                StackTrace = ContentMatch.Value.Trim();
            }
        }
    }

    private void ParseExtendedProperties(string exprop)
    {
        // outer message
        var ContentMatch = Regex.Match(exprop, exPropRegex, RegexOptions.Multiline);
        while (ContentMatch.Success)
        {
            var key = ContentMatch.Groups["key"].Value.Trim();
            var value = ContentMatch.Groups["value"].Value.Trim();
            ExtendedProperties.Add(key, value);
            ContentMatch = ContentMatch.NextMatch();
        }

        if (ExtendedProperties.TryGetValue("Http Request Url", out var value2))
        {
            RequestUrl = value2;
        }
    }
}