using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using Serilog;

namespace EventLogAnalysis;

public class ELog : LogBase<ELRecord>
{
    private LogEntryCollection<ELRecord> filteredEvents = new();

    private string StatusCompleted = string.Empty;

    private string StatusInProgress = string.Empty;

    public ELog(string filename)
    {
        SourceName = filename;
        fileInfo = new FileInfo(FileName);
        MtaFilePath = GetMtaFilePath(fileInfo);

        ProvidersNotToFormat.Add("KofaxTransformationServerService");
        ProvidersNotToFormat.Add("TotalAgility");
        ProvidersNotToFormat.Add("Total Agility");

        TypeName = "Event Log (evtx)";
    }

    public Dictionary<string, int> AllProviders { get; private set; } = new();
    public override ILogEntryCollection<ELRecord> EntryCollection => filteredEvents;
    public int EventsWithMessagesLoaded => UnfilteredEvents.Lines.Where(x => x.MessageIsLoaded).Count();
    public string FileName => SourceName;
    public long FilteredEventCount => currentFilterEventIndexes.Count;
    public Dictionary<string, int> FilteredProviders { get; private set; } = new();
    public List<string> ProvidersNotToFormat { get; private set; } = new();
    public LogEntryCollection<ELRecord> RecordCollection => filteredEvents;
    public long TotalEventCount { get; private set; }
    public List<LogBasedTraitProducer> TraitProducers { get; } = new();
    public LogEntryCollection<ELRecord> UnfilteredEvents { get; private set; } = new();
    private List<long> currentFilterEventIndexes { get; set; } = new();
    private FileInfo fileInfo { get; init; }

    private string MtaFilePath { get; }

    public void CreateLogBasedTraitProducers()
    {
        // do this via plugin or whatever in the future
        TraitProducers.Add(new LogBasedTraitProducer());
    }

    //public void Filter(string xpath)
    //{
    //    var eq = new EventLogQuery(FileName, PathType.FilePath, xpath);
    //    LoadEvents(eq);
    //    UpdateProvidersFromEvents(FilteredProviders, EntryCollection);
    //}

    public override void InitialLoad(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
    {
        var stopwatch = Stopwatch.StartNew();
        RenameMtaFileIfPossible(false);
        var eq = new EventLogQuery(FileName, PathType.FilePath);
        //SaveCopy(eq);
        LoadEvents(eq, cancelToken, progress);
        TotalEventCount = UnfilteredEvents.Lines.Count();

        UpdateProvidersFromEvents(AllProviders, UnfilteredEvents);
        FilteredProviders = new(AllProviders);

        // remove in future
        CreateLogBasedTraitProducers();

        RenameMtaFileIfPossible(true);

        StatusCompleted = $"{TotalEventCount} events loaded";
        StatusInProgress = string.Empty;
        InternalLog.WriteLine($"Finished loading events of log ({stopwatch.ElapsedMilliseconds:n0} ms): {FileName}");
    }

    public override void LoadMessages(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
    {
        var stopwatch = Stopwatch.StartNew();

        if (TotalEventCount == 0) { InitialLoad(cancelToken, progress); }

        RenameMtaFileIfPossible(false);

        //Workaround: http://stackoverflow.com/questions/7531557/why-does-eventrecord-formatdescription-return-null
        var OriginalCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        using var dt = new DisposableTrace(Label: $"{UnfilteredEvents.Lines.Count()} messages from {fileInfo.Name}");

        int loadedMessages = 0;
        //load in reverse to get newset events first
        foreach (var e in RecordCollection.EntryList.FastReverse())
        {
            e.GetMessage();
            loadedMessages++;

            if (loadedMessages % 1000 == 0)
            {
                StatusInProgress = $"{loadedMessages / (double)RecordCollection.EntryList.Count:P0} ({loadedMessages} / {RecordCollection.EntryList.Count}) loaded event messages...";

                progress.Report(new ProgressUpdate() { RefreshLogsView = true });

                if (cancelToken.IsCancellationRequested)
                {
                    Thread.CurrentThread.CurrentCulture = OriginalCulture;
                    return;
                }
            }
        }

        // Parallel message loading does not help... must be a lock in the whole log when getting a message.
        //Parallel.ForEach(FilteredEvents, e => e.GetMessage());

        StatusCompleted = $"{TotalEventCount} event messages loaded";
        StatusInProgress = string.Empty;

        InternalLog.WriteLine($"Finished loading messages of log ({stopwatch.ElapsedMilliseconds:n0} ms): {FileName}");
        progress.Report(new ProgressUpdate(true, string.Empty));

        Thread.CurrentThread.CurrentCulture = OriginalCulture;
        RenameMtaFileIfPossible(true);
    }

    public override void LoadTraits(CancellationToken cancelToken)
    {
        Traits = new();

        LoadTraitsPerLine();
    }

    public override string LogStatus()
    {
        return $"{StatusCompleted}  {StatusInProgress}";
    }

    private string GetMtaFilePath(FileInfo evtx)
    {
        // changing from Join (.net core only) to Combine.  Not sure if relevant:
        // Unlike the Combine method, the Join method does not attempt to root the returned path.
        // (That is, if path2 is an absolute path, the Join method does not discard the previous paths as the Combine method does.
        string folder = Path.Combine(evtx.DirectoryName!, "LocaleMetaData");
        string filename = Path.GetFileNameWithoutExtension(evtx.Name) + "_1033.MTA";
        return Path.Combine(folder, filename);
    }

    private void LoadComplexTraits(CancellationToken cancelToken)
    {
    }

    /// <summary>
    /// Load the actual event objects that match a query, without loading event messages
    /// </summary>
    /// <param name="query"></param>
    private void LoadEvents(EventLogQuery query, CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
    {
        using var dt = new DisposableTrace($"Loading events from {fileInfo.Name}");

        currentFilterEventIndexes.Clear();
        RecordCollection.Clear();

        int loadedEvents = 0;
        using (EventLogReader reader = new EventLogReader(query))
        {
            EventRecord ev;
            while ((ev = reader.ReadEvent()) != null)
            {
                var elr = new ELRecord(ev, this);
                loadedEvents++;

                // after initial load these are ignored
                UnfilteredEvents.Add(elr);

                // filtered events are cleared at start of each load events
                RecordCollection.Add(elr);

                currentFilterEventIndexes.Add(elr.RecordId);

                if (loadedEvents % 1000 == 0)
                {
                    StatusInProgress = $"{loadedEvents} loaded events...";

                    progress.Report(new ProgressUpdate() { RefreshLogsView = true });

                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }
    }

    private void LoadTraitsPerLine()
    {
        // to be plugin based etc at some point
        List<LineBasedTraitProducer> producers = new();
        producers.Add(new LineBasedTraitProducer());

        foreach (var r in RecordCollection.Lines)
        {
            foreach (var p in producers)
            {
                p.AddTraitsFromLine(Traits, r);
            }
        }
    }

    private void RenameMtaFileIfPossible(bool backToOriginal)
    {
        void RenameIfPossible(string from, string to)
        {
            try
            {
                File.Move(from, to);
            }
            catch (Exception)
            {
            }
        }

        if (Options.Instance.RenameMtaDuringLoad)
        {
            if (backToOriginal)
            {
                RenameIfPossible($"{MtaFilePath}.tmp", MtaFilePath);
            }
            else
            {
                RenameIfPossible(MtaFilePath, $"{MtaFilePath}.tmp");
            }
        }
    }

    //private void SaveCopy(EventLogQuery eq)
    //{
    //    var stopwatch = Stopwatch.StartNew();
    //    var newFile = $"{FileName}-tmp.evtx";

    //    eq.Session.ExportLogAndMessages(FileName, PathType.FilePath, "", newFile);
    //    InternalLog.WriteLine($"Finished saving events of log ({stopwatch.ElapsedMilliseconds:n0} ms): {newFile}");
    //}

    private void UpdateProvidersFromEvents(Dictionary<string, int> providers, LogEntryCollection<ELRecord> events)
    {
        foreach (var r in events.Entries)
        {
            if (!providers.TryAdd(r.Record.ProviderName, 1))
            {
                providers[r.Record.ProviderName]++;
            }
        }
    }
}