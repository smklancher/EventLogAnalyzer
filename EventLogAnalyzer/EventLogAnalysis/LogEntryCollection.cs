namespace EventLogAnalysis;

public class LogEntryCollection<T> : ILogEntryCollection<T> where T : LogEntry
{
    public virtual ILogEntryCollection<LogEntry> AsLogEntries => this;
    public virtual IEnumerable<T> Entries => EntryList;

    /// <summary>
    /// Presumably this isn't needed since this object itself can already cast to ILogEntryCollection<LogEntry>
    /// </summary>
    public ILogEntryCollection<LogEntry> EntriesGeneric => this;

    public DictionaryOnInsertButSortedListOnAccess<T> EntryList { get; } = new();

    // Thought this approach was needed but guess not
    // public ILogEntryCollection<LogEntry> EntriesGeneric => ((ILogEntryCollection<LogEntry>)this).AsLogEntries;

    public virtual DateTime FirstEvent => Entries.FirstOrDefault()?.Timestamp ?? DateTime.MinValue;

    public virtual DateTime LastEvent => Entries.LastOrDefault()?.Timestamp ?? DateTime.MaxValue;

    //convenience for old code
    public IEnumerable<T> Lines => Entries;

    /// <summary>
    /// Add item by UniqueID, duplicates ignored
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        EntryList.Add(item, item.UniqueId);
    }

    public void Clear()
    {
        EntryList.Clear();
    }

    public virtual ILogEntryCollection<T> FilteredCopy(string FilterText)
    {
        var lc = new LogEntryCollection<T>();
        foreach (var l in Entries.Where(x => x.Message.ToLower().Contains(FilterText.ToLower())))
        {
            lc.Add(l);
        }

        lc.EntryList.Lock();
        return lc;
    }

    //public virtual ILogEntryCollection<T> FilteredCopy(FilterOptions filterOptions)
    //{
    //    var lc = new LogEntryCollection<T>();

    //    var includes = filterOptions.Includes;
    //    var excludes = filterOptions.Excludes;

    //    foreach (var e in Entries)
    //    {
    //        var isIncluded = true;

    //        if (includes.Count > 0)
    //        {
    //            if (Options.Instance.CsvSearchWithOR)
    //            {
    //                isIncluded = includes.Any(x => e.Message.ToLower().Contains(x));
    //            }
    //            else
    //            {
    //                isIncluded = includes.All(x => e.Message.ToLower().Contains(x));
    //            }
    //        }

    //        if (isIncluded)
    //        {
    //            var isExcluded = excludes.Any(x => e.Message.ToLower().Contains(x));

    //            if (!isExcluded)
    //            {
    //                lc.Add(e);
    //            }
    //        }
    //    }

    //    lc.EntryList.Lock();
    //    return lc;
    //}

    public virtual ILogEntryCollection<T> FilteredCopy(FilterOptions filterOptions)
    {
        // Consider allowing defined AND/OR filter sets
        // https://enlear.academy/building-a-dynamic-logical-expression-builder-in-c-c8f998451334

        var lc = new LogEntryCollection<T>();

        foreach (var e in Entries)
        {
            // if no include filters, then everything is included
            var isIncluded = filterOptions.IncludeFilters.Count == 0;

            // otherwise include if it meets an include filter
            foreach (var f in filterOptions.IncludeFilters)
            {
                if (f.AppliesTo.IsAssignableFrom(e.GetType()))
                {
                    isIncluded = f.TestObject(e);

                    // if any include filter met, skip remaining
                    if (isIncluded) { break; }
                }
            }

            var isExcluded = false;
            // if included, check exclude filters
            if (isIncluded)
            {
                foreach (var f in filterOptions.ExcludeFilters)
                {
                    if (f.AppliesTo.IsAssignableFrom(e.GetType()))
                    {
                        isExcluded = f.TestObject(e);

                        // if any exclude filter met, skip remaining
                        if (isExcluded) { break; }
                    }
                }
            }

            if (isIncluded && !isExcluded)
            {
                // if it is included and not excluded, check any highlight filters
                var highlighted = false;
                e.Color = null;

                foreach (var f in filterOptions.HighlightFilters)
                {
                    if (f.AppliesTo.IsAssignableFrom(e.GetType()))
                    {
                        highlighted = f.TestObject(e);

                        // if any highlighted filter met, skip remaining
                        if (highlighted) { e.Color = f.HighlightColor; break; }
                    }
                }

                lc.Add(e);
            }
        }

        lc.EntryList.Lock();
        return lc;
    }

    /// <summary>
    /// Merges another LineCollection into this one
    /// </summary>
    /// <param name="CollectionToMerge"></param>
    /// <remarks></remarks>
    public void Merge(LogEntryCollection<T> CollectionToMerge)
    {
        foreach (var LineToMerge in CollectionToMerge.Entries)
        {
            Add(LineToMerge);
        }
    }
}