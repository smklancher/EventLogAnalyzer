﻿using System;
using System.Diagnostics.Eventing.Reader;

namespace EventLogAnalysis
{
    public class ELRecord : IComparable<ELRecord>, IEquatable<ELRecord>
    {
        public ELRecord(EventRecord eventRecord, ELog log)
        {
            Record = eventRecord;
            ParentLog = log;

            GroupKey = new ProviderEventIdPair(Record.ProviderName, Record.Id);

            // not sure what conditions under which it can be null, need to run into it to find out
            RecordId = (long)Record.RecordId!;

            UniqueId = $"{ParentLog.LogGuid}-{RecordId}";

            EventLevel = (StandardEventLevel)(Record.Level ?? 4); //intern these strings
            Level = EventLevel.ToString();
        }

        public bool AltMessage { get; private set; } = false;
        public StandardEventLevel EventLevel { get; }
        public ProviderEventIdPair GroupKey { get; init; }
        public double GroupSimilarity { get; set; }
        public string Level { get; }
        public string LogFileName => ParentLog.FileName;

        /// <summary>
        /// Message or empty string if not loaded
        /// </summary>
        public string Message { get; private set; } = string.Empty;

        public bool MessageIsLoaded { get; private set; }
        public ELog ParentLog { get; }
        public string ProviderName => Record.ProviderName;
        public EventRecord Record { get; init; }
        public long RecordId { get; private set; }
        public DateTime? Timestamp => Record.TimeCreated;
        public string UniqueId { get; }

        public int CompareTo(ELRecord? other)
        {
            return (Timestamp ?? DateTime.MaxValue).CompareTo(other?.Timestamp ?? DateTime.MaxValue);
        }

        public bool Equals(ELRecord? other)
        {
            if (other is null) { return false; }
            if (object.ReferenceEquals(this, other)) { return true; }

            return UniqueId == other.UniqueId;
        }

        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }

        public string GetMessage()
        {
            if (!MessageIsLoaded)
            {
                Message = Record.FormatDescription();
                if (string.IsNullOrEmpty(Message))
                {
                    if (Record.Properties.Count > 0)
                    {
                        Message = Record.Properties[0].Value.ToString() ?? string.Empty;
                        AltMessage = true;
                    }
                    else
                    {
                        Message = string.Empty;
                        AltMessage = true;
                    }
                }
                MessageIsLoaded = true;
            }

            return Message;
        }
    }
}