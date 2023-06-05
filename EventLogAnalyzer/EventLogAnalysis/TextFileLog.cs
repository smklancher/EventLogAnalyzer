using Serilog;

namespace EventLogAnalysis;

// Future consideration: try different file reading approaches
// maybe using Memory/Span, and esp when regex accepts those directly:
// https://github.com/dotnet/runtime/issues/59629
// some things like this would need to be different or not available if targeting .NET 4.8
// https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries#how-to-multitarget
// large file mode:
// index linebreak offsets and then lines by random access on demand

public class TextFileLog : LogBase<TextFileLogEntry>
{
    private const int MaxInvalidStartingLines = 25;
    private FileInfo fileInfo;

    public TextFileLog(string filename)
    {
        SourceName = filename;
        fileInfo = new FileInfo(filename);

        TypeName = "Test Log Type";
    }

    public TextFileLogEntryCollection Entries { get; private set; } = new();

    public override ILogEntryCollection<TextFileLogEntry> EntryCollection => Entries;

    public List<string> LogList { get; private set; } = new();

    public override void InitialLoad(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
    {
        base.InitialLoad(cancelToken, progress);

        // consider indexing lines here, possibly a separate mode when files are large?
        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/d47c3e79-08b0-40a9-9fb7-af683c40d755/tip-1-index-lines-in-large-text-file-for-fast-random-access?forum=vbgeneral
    }

    public override void LoadMessages(CancellationToken cancelToken, IProgress<ProgressUpdate> progress)
    {
        TextFileLogEntry? MostRecentRealLine = null;

        using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            // The Using statement for the FileStream is all that is needed, see http://stackoverflow.com/a/12000155/221018
            StreamReader sr = new StreamReader(fs);

            long LineNumber = 0;
            TextFileLogEntry? LastLine = EntryCollection.Entries.LastOrDefault();
            long LinesAlreadyLoaded = 0;
            if (LastLine != null)
            {
                LinesAlreadyLoaded = LastLine.LineNumber + LastLine.DependentLineCount;
            }

            // Lines are only added once all continuations are added because the extra lookups from doing it
            // after the fact made Load take 50% longer
            // Dim MostRecentRealLine As LogLine = Nothing
            TextFileLogEntry? CurrentLine = null;
            while (!sr.EndOfStream)
            {
                LineNumber = LineNumber + 1;
                string line = sr.ReadLine()!;

                // Always skip the number of lines which are already loaded, so this can be called initially or as an update
                if (LineNumber > LinesAlreadyLoaded)
                {
                    /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped ElseDirectiveTrivia */
                    CurrentLine = CreateLine(line, LineNumber);

                    if (CurrentLine is null) { continue; }

                    // TODO: still need to figure out the best way to handle errors without hidding them
                    // Try
                    // CurrentLine = CreateLine(line, LineNumber)
                    // Catch ex As Exception
                    // 'Any uncaught exception while creating the line causes an unknown line to be created as a parse error
                    // 'CurrentLine = New UnknownLine(line & vbNewLine & vbNewLine & "ParseError: " & ex.ToString, LineNumber, Me, LogLine.LogLevel.ParseError)
                    // End Try
                    /* TODO ERROR: Skipped EndIfDirectiveTrivia */
                    // Decide what to do based on whether this is a new real line or a dependent line
                    if (CurrentLine.ContinuesPreviousLine)
                    {
                        // Ignore invalid starting lines (continuing when there isn't a real line yet).
                        if (MostRecentRealLine != null)
                        {
                            // append this continued line to the most recent real line
                            // MostRecentRealLine.AppendMessage(CurrentLine.Message)

                            // add this as a dependent line
                            // TODO: still need to figure out the best way to handle errors without hidding them
                            // AddDependentLine similar to CreateLine
                            MostRecentRealLine.AddDependentLine(CurrentLine);
                        }
                        else if (LineNumber > MaxInvalidStartingLines)
                        {
                            // This will prevent the whole file from being read if all lines are ContinuesPreviousLine or ParseError
                            Log.Warning($"Quitting parse attempt: No valid parse after {MaxInvalidStartingLines} lines. ({GetType()}, {SourceName})");
                            break;
                        }
                    }
                    else
                    {
                        // track number of parse errors
                        //if (CurrentLine.Level == LogLine.LogLevel.ParseError)
                        //{
                        //    mParseErrorCount = mParseErrorCount + 1;
                        //}

                        //if (OnlyLoadFirst)
                        //{
                        //    // the last (and only) line will get added after the while loop
                        //    break;
                        //}

                        if (MostRecentRealLine != null)
                        {
                            // Add the most recent real line now that we found the start of a new one
                            CompleteLineParsed(MostRecentRealLine);
                        }

                        // Now the current line is the most recent
                        MostRecentRealLine = CurrentLine;
                    }
                }
            }

            if (MostRecentRealLine != null)
            {
                // Add the last line after the loop is done
                CompleteLineParsed(MostRecentRealLine);
            }
            else if (CurrentLine != null)
            {
                // This will only happen if all lines are marked as ContinuesPreviousLine
                Entries.Add(CurrentLine);
            }
            else
            {
            }
        }
    }

    private void CompleteLineParsed(TextFileLogEntry Line)
    {
        //BeforeAddLine(Line);

        //LineParsed?.Invoke(this, new LineParsedEventArgs(Line, this));

        //if (!DiscardLinesAfterParse)
        Entries.Add(Line);
    }

    private TextFileLogEntry? CreateLine(string line, long lineNumber)
    {
        return new TextFileLogEntry(line);
    }
}

public class TextFileLogEntry : LogEntry
{
    public TextFileLogEntry(string msg)
    {
        Message = msg;
    }

    public bool ContinuesPreviousLine { get; internal set; }
    public int DependentLineCount { get; internal set; }
    public int LineNumber { get; internal set; }

    internal void AddDependentLine(TextFileLogEntry currentLine)
    {
        throw new NotImplementedException();
    }
}

public class TextFileLogEntryCollection : LogEntryCollection<TextFileLogEntry>
{
    public TextFileLogEntryCollection()
    {
    }
}