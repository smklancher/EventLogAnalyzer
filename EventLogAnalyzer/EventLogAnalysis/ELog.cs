using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Threading;
using Serilog;
using Similarity;
using UtilityCommon;

namespace EventLogAnalysis
{
    public class ELog
    {
        public ELog(string filename)
        {
            FileName = filename;
            LogGuid = new();
            fileInfo = new FileInfo(FileName);
            MtaFilePath = GetMtaFilePath(fileInfo);

            RenameMtaFileIfPossible(false);
            var eq = new EventLogQuery(FileName, PathType.FilePath);
            LoadEvents(eq);
            TotalEventCount = AllEvents.Count;

            UpdateProvidersFromEvents(AllProviders, AllEvents);
            FilteredProviders = new(AllProviders);

            // remove in future
            CreateLogBasedTraitProducers();

            RenameMtaFileIfPossible(true);
        }

        public EventCollection AllEvents { get; private set; } = new();

        public Dictionary<string, int> AllProviders { get; private set; } = new();

        public Dictionary<ProviderEventIdPair, EventIdGroup> EventIdGroups { get; private set; } = new();

        public int EventsWithMessagesLoaded => AllEvents.Where(x => x.MessageIsLoaded).Count();

        public string FileName { get; init; }

        public long FilteredEventCount => currentFilterEventIndexes.Count;

        public EventCollection FilteredEvents { get; private set; } = new();

        public Dictionary<string, int> FilteredProviders { get; private set; } = new();

        public Guid LogGuid { get; }

        public WorkingSetGroup<ELRecord>? SimilarityGroups { get; private set; }

        public long TotalEventCount { get; private set; }

        public List<LogBasedTraitProducer> TraitProducers { get; } = new();

        public TraitTypeCollection Traits { get; private set; } = new();

        private List<long> currentFilterEventIndexes { get; set; } = new();

        private FileInfo fileInfo { get; init; }

        private string MtaFilePath { get; }

        ///// <summary>
        ///// Add all lines into groups for that event type
        ///// </summary>
        ///// <param name="log"></param>
        ///// <returns></returns>
        //public static Dictionary<ProviderEventIdPair, EventIdGroup> CreateEventIdGroups(ELog log)
        //{
        //    using var dt = new DisposableTrace();

        //    Dictionary<ProviderEventIdPair, EventIdGroup> groups = new();
        //    foreach (var line in log.FilteredEvents)
        //    {
        //        if (!groups.ContainsKey(line.GroupKey))
        //        {
        //            // first of this kind of event, so create a new line group
        //            var group = new EventIdGroup(line.GroupKey);
        //            groups.Add(group.GroupKey, group);
        //        }

        //        groups[line.GroupKey].Records.Add(line);
        //    }
        //    return groups;
        //}

        public void CreateLogBasedTraitProducers()
        {
            // do this via plugin or whatever in the future
            TraitProducers.Add(new LogBasedTraitProducer());
        }

        public void Filter(string xpath)
        {
            var eq = new EventLogQuery(FileName, PathType.FilePath, xpath);
            LoadEvents(eq);
            UpdateProvidersFromEvents(FilteredProviders, FilteredEvents);
        }

        public void LoadMessages(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
        {
            var stopwatch = Stopwatch.StartNew();

            RenameMtaFileIfPossible(false);

            //Workaround: http://stackoverflow.com/questions/7531557/why-does-eventrecord-formatdescription-return-null
            var OriginalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            using var dt = new DisposableTrace(Label: $"{AllEvents.Count} messages from {fileInfo.Name}");
            foreach (var e in FilteredEvents)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    Thread.CurrentThread.CurrentCulture = OriginalCulture;
                    return;
                }

                e.GetMessage();
            }

            Log.Information($"Finished loading messages of log ({stopwatch.ElapsedMilliseconds:n0} ms): {FileName}");
            progress.Report(new ProgressUpdate(true, string.Empty));

            Thread.CurrentThread.CurrentCulture = OriginalCulture;
            RenameMtaFileIfPossible(true);
        }

        public void LoadTraits(CancellationToken cancelToken)
        {
            Traits = new();

            LoadTraitsPerLine();

            // create index for similar lines, ideally standardize index approach
            SimilarityGroups = Processing.Process(
                FilteredEvents,
                Options.Instance.SimilarityOptions.LinesPerSimilarityGroupChunk,
                x => string.IsNullOrWhiteSpace(x.ShortMessage) ? x.Message : x.ShortMessage);
        }

        ///// <summary>
        ///// Need to change to parallel with cancelation
        ///// Groups similar lines within the EventIdGroups
        ///// </summary>
        ///// <param name="LineGroupings"></param>
        //private static void GroupSimilarLines(Dictionary<ProviderEventIdPair, EventIdGroup> LineGroupings)
        //{
        //    using var dt = new DisposableTrace();
        //    foreach (var g in LineGroupings.Values)
        //    {
        //        g.GroupSimilarLines();
        //    }
        //}

        private string GetMtaFilePath(FileInfo evtx)
        {
            string folder = Path.Join(evtx.DirectoryName!, "LocaleMetaData");
            string filename = Path.GetFileNameWithoutExtension(evtx.Name) + "_1033.MTA";
            return Path.Join(folder, filename);
        }

        private void LoadComplexTraits(CancellationToken cancelToken)
        {
        }

        /// <summary>
        /// Load the actual event objects that match a query, without loading event messages
        /// </summary>
        /// <param name="query"></param>
        private void LoadEvents(EventLogQuery query)
        {
            using var dt = new DisposableTrace($"Loading events from {fileInfo.Name}");

            currentFilterEventIndexes.Clear();
            FilteredEvents.Clear();

            using (EventLogReader reader = new EventLogReader(query))
            {
                EventRecord ev;
                while ((ev = reader.ReadEvent()) != null)
                {
                    var elr = new ELRecord(ev, this);

                    // after initial load these are ignored
                    AllEvents.Add(elr);

                    // filtered events are cleared at start of each load events
                    FilteredEvents.Add(elr);

                    currentFilterEventIndexes.Add(elr.RecordId);
                }
            }
        }

        private void LoadTraitsPerLine()
        {
            // to be plugin based etc at some point
            List<LineBasedTraitProducer> producers = new();
            producers.Add(new LineBasedTraitProducer());

            foreach (var r in FilteredEvents)
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

        private void UpdateProvidersFromEvents(Dictionary<string, int> providers, EventCollection events)
        {
            foreach (var r in events)
            {
                if (!providers.TryAdd(r.Record.ProviderName, 1))
                {
                    providers[r.Record.ProviderName]++;
                }
            }
        }
    }
}