using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using Serilog;

namespace EventLogAnalysis;

public record ProviderEventIdPair(string Provider, int EventId);

public class ELRecord : LogEntry
{
    private string xml = string.Empty;

    public ELRecord(EventRecord eventRecord, ELog log)
    {
        Record = eventRecord;
        ParentLog = log;

        GroupKey = new ProviderEventIdPair(Record.ProviderName, Record.Id);
        Timestamp = Record.TimeCreated;

        // not sure what conditions under which it can be null, need to run into it to find out
        RecordId = (long)Record.RecordId!;

        UniqueId = $"{ParentLog.LogGuid}-{RecordId}";

        EventLevel = (StandardEventLevel)(Record.Level ?? 4); //intern these strings
        Level = EventLevel.ToString();
    }

    public StandardEventLevel EventLevel { get; }

    public ProviderEventIdPair GroupKey { get; init; }

    public double GroupSimilarity { get; set; }

    public string LogFileName => ParentLog.FileName;

    public bool MessageIsLoaded { get; private set; }

    public Exception? MessageLoadExeption { get; private set; }

    public ELog ParentLog { get; }

    public string ProviderName => Record.ProviderName;

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public EventRecord Record { get; init; }

    public long RecordId { get; private set; }

    public string Xml
    {
        get
        {
            if (xml == string.Empty)
            {
                xml = Record.ToXml();
            }
            return xml;
        }
    }

    public string GetMessage()
    {
        if (!MessageIsLoaded)
        {
            bool skipFormat = ProviderDoesNotUseFormattedMessages(Record.ProviderName);
            if (Options.Instance.SkipFormattedMessage || skipFormat)
            {
                Message = PropertiesAsString();
            }
            else
            {
                try
                {
                    Message = Record.FormatDescription();
                }
                catch (Exception ex)
                {
                    // TODO: check how different kind of exceptions should be handled, eg ERROR_EVT_INVALID_PUBLISHER_NAME
                    //  https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/18d8fbe8-a967-4f1c-ae50-99ca8e491d2d
                    MessageLoadExeption = ex;
                }

                if (string.IsNullOrEmpty(Message))
                {
                    if (!skipFormat)
                    {
                        InternalLog.WriteLine($"Skipping subsequent formatting for provider {Record.ProviderName} due to empty message.");
                        if (MessageLoadExeption is not null)
                        { InternalLog.WriteLine($"Additionally format attempt with provider {Record.ProviderName} threw error: {MessageLoadExeption.Message}."); }
                        ParentLog.ProvidersNotToFormat.Add(Record.ProviderName);
                    }

                    Message = PropertiesAsString();
                }
            }

            MessageIsLoaded = true;
        }

        return Message;
    }

    public string PropertiesAsString()
    {
        if (Record.Properties.Count == 0) { return string.Empty; }
        if (Record.Properties.Count == 1) { return Record.Properties[0]?.Value?.ToString() ?? string.Empty; }

        var sb = new StringBuilder();
        int propIndex = 0;
        foreach (var p in Record.Properties)
        {
            sb.AppendLine($"[{propIndex}] {p.Value}");
            propIndex++;
        }

        return sb.ToString();
    }

    public bool ProviderDoesNotUseFormattedMessages(string providerName) => ParentLog.ProvidersNotToFormat.Contains(providerName);
}