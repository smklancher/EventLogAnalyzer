//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Security;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.VisualBasic;
//using System.ComponentModel.Composition;
//using System.Runtime.InteropServices;

//[InheritedExport(typeof(LogFile))]
//public abstract class LogFile
//{
//    private const int MaxInvalidStartingLines = 25;
//    private const string NormalizedIndexName = "UniqueMessages";

//    public Dictionary<string, LogIndexAttribute> LogIndexInfo = new Dictionary<string, LogIndexAttribute>();

//    public event LineParsedEventHandler LineParsed;

//    public delegate void LineParsedEventHandler(object sender, LineParsedEventArgs e);

//    public bool DiscardLinesAfterParse { get; set; } = false;

//    public virtual string MachineName { get; set; } = "";

//    // apparently FileInfo.FullName is pretty expensive
//    public string FileName { get; set; }

//    protected FileInfo mFileObj;

//    public FileInfo File
//    {
//        get
//        {
//            return mFileObj;
//        }
//        set
//        {
//            mFileObj = value;
//            FileName = value.FullName;
//        }
//    }

//    protected LineCollection mLines = new LineCollection();

//    public LineCollection Lines
//    {
//        get
//        {
//            return mLines;
//        }
//    }

//    protected Nullable<DateTime> mTimestampStart;
//    protected Nullable<DateTime> mTimestampEnd;

//    public long LineCount
//    {
//        get
//        {
//            return mLines.LongCount;
//        }
//    }

//    protected long mParseErrorCount;

//    public long ParseErrorCount
//    {
//        get
//        {
//            return mParseErrorCount;
//        }
//    }

//    protected Nullable<DateTime> mDate;

//    public Nullable<DateTime> LogDate
//    {
//        get
//        {
//            return mDate;
//        }
//    }

//    /// <summary>
//    ///     ''' Parse the log, populating the LinesCollection
//    ///     ''' </summary>
//    ///     ''' <remarks></remarks>
//    public virtual void Load(bool OnlyLoadFirst = false)
//    {
//        LogLine MostRecentRealLine = null/* TODO Change to default(_) if this is not a reference type */;

//        using (FileStream fs = new FileStream(File.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//        {
//            // The Using statement for the FileStream is all that is needed, see http://stackoverflow.com/a/12000155/221018
//            StreamReader sr = new StreamReader(fs);

//            long LineNumber = 0;
//            LogLine LastLine = mLines.LastOrDefault;
//            long LinesAlreadyLoaded = 0;
//            if (LastLine != null)
//                LinesAlreadyLoaded = LastLine.LineNumber + LastLine.DependentLineCount;

//            // Lines are only added once all continuations are added because the extra lookups from doing it
//            // after the fact made Load take 50% longer
//            // Dim MostRecentRealLine As LogLine = Nothing
//            LogLine CurrentLine = null/* TODO Change to default(_) if this is not a reference type */;
//            while (!sr.EndOfStream)
//            {
//                LineNumber = LineNumber + 1;
//                string line = sr.ReadLine();

//                // Always skip the number of lines which are already loaded, so this can be called initially or as an update
//                if (LineNumber > LinesAlreadyLoaded)
//                {
//                    /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped ElseDirectiveTrivia */
//                    CurrentLine = CreateLine(line, LineNumber);

//                    // TODO: still need to figure out the best way to handle errors without hidding them
//                    // Try
//                    // CurrentLine = CreateLine(line, LineNumber)
//                    // Catch ex As Exception
//                    // 'Any uncaught exception while creating the line causes an unknown line to be created as a parse error
//                    // 'CurrentLine = New UnknownLine(line & vbNewLine & vbNewLine & "ParseError: " & ex.ToString, LineNumber, Me, LogLine.LogLevel.ParseError)
//                    // End Try
//                    /* TODO ERROR: Skipped EndIfDirectiveTrivia */
//                    // Decide what to do based on whether this is a new real line or a dependent line
//                    if (CurrentLine.Level == LogLine.LogLevel.ContinuesPreviousLine | CurrentLine.Level == LogLine.LogLevel.Blank | CurrentLine.Level == LogLine.LogLevel.Ignore)
//                    {
//                        // Ignore invalid starting lines (continuing when there isn't a real line yet).
//                        if (MostRecentRealLine != null)
//                            // append this continued line to the most recent real line
//                            // MostRecentRealLine.AppendMessage(CurrentLine.Message)

//                            // add this as a dependent line
//                            // TODO: still need to figure out the best way to handle errors without hidding them
//                            // AddDependentLine similar to CreateLine
//                            MostRecentRealLine.AddDependentLine(CurrentLine);
//                        else if (LineNumber > MaxInvalidStartingLines)
//                        {
//                            // This will prevent the whole file from being read if all lines are ContinuesPreviousLine or ParseError
//                            LogCollection.LogMsg($"Quitting parse attempt: No valid parse after {MaxInvalidStartingLines} lines. ({GetType.ToString()}, {FileName})", LogLine.LogLevel.Error);
//                            break;
//                        }
//                    }
//                    else
//                    {
//                        // track number of parse errors
//                        if (CurrentLine.Level == LogLine.LogLevel.ParseError)
//                            mParseErrorCount = mParseErrorCount + 1;

//                        if (OnlyLoadFirst)
//                            // the last (and only) line will get added after the while loop
//                            break;

//                        if (MostRecentRealLine != null)
//                            // Add the most recent real line now that we found the start of a new one
//                            CompleteLineParsed(MostRecentRealLine);

//                        // Now the current line is the most recent
//                        MostRecentRealLine = CurrentLine;
//                    }
//                }
//            }

//            if (MostRecentRealLine != null)
//                // Add the last line after the loop is done
//                CompleteLineParsed(MostRecentRealLine);
//            else if (CurrentLine != null)
//                // This will only happen if all lines are marked as ContinuesPreviousLine
//                mLines.Add(CurrentLine);
//            else
//            {
//            }
//        }
//    }

//    private void CompleteLineParsed(LogLine Line)
//    {
//        BeforeAddLine(Line);

//        LineParsed?.Invoke(this, new LineParsedEventArgs(Line, this));

//        if (!DiscardLinesAfterParse)
//            mLines.Add(Line);
//    }

//    public virtual string Details()
//    {
//        return ""; // Marshal.SizeOf(Me).ToString
//    }

//    /// <summary>
//    ///     ''' This is called before a line is added to the log during load so that the child class has a chance to use it for anything.
//    ///     ''' </summary>
//    ///     ''' <param name="NewLine"></param>
//    ///     ''' <remarks></remarks>
//    public virtual void BeforeAddLine(LogLine NewLine)
//    {
//    }

//    /// <summary>
//    ///     ''' This is called after indicies for the log collection have been compiled.  This allows for adding meta-indicies based on primary indicies.
//    ///     ''' </summary>
//    ///     ''' <param name="IdxCol"></param>
//    public virtual void AddIndicies(IndexCollection IdxCol)
//    {
//    }

//    /// <summary>
//    ///     ''' Parse a date from an exact format or return a null date
//    ///     ''' </summary>
//    ///     ''' <param name="DateString"></param>
//    ///     ''' <param name="FormatString"></param>
//    ///     ''' <returns></returns>
//    ///     ''' <remarks></remarks>
//    public static Nullable<DateTime> ParseExactNullableDate(string DateString, string FormatString)
//    {
//        DateTime NonNullableDate;
//        if (DateTime.TryParseExact(DateString, FormatString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowTrailingWhite, ref NonNullableDate))
//            return NonNullableDate;
//        return default(Date?);
//    }

//    /// <summary>
//    ///     ''' Parse a date from an exact format or return a null date
//    ///     ''' </summary>
//    ///     ''' <param name="DateString"></param>
//    ///     ''' <returns></returns>
//    ///     ''' <remarks></remarks>
//    public static Nullable<DateTime> ParseNullableDate(string DateString)
//    {
//        DateTime NonNullableDate;
//        if (DateTime.TryParse(DateString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowTrailingWhite, ref NonNullableDate))
//            return NonNullableDate;
//        return default(Date?);
//    }

//    private IndexCollection mIndicies = new IndexCollection();

//    /// <summary>
//    ///     ''' Indicies for this log file
//    ///     ''' </summary>
//    ///     ''' <returns></returns>
//    ///     ''' <remarks></remarks>
//    public IndexCollection Indicies()
//    {
//        mIndicies = new IndexCollection();

//        // 'go through each line
//        foreach (LogLine CurLine in mLines)
//        {
//            Dictionary<string, LogIndexAttribute> IndexDelegateSet = null;

//            LogIndexInfo = DelegateCreator.IndexDelegateSet(CurLine.GetType);

//            foreach (var kvp in LogIndexInfo)
//            {
//                LogIndexAttribute LogIndex = kvp.Value;
//                string IndexValue = "";

//                // Ignore special case index that needs to run after all others
//                if (LogIndex.IndexName == NormalizedIndexName)
//                    continue;

//                /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped ElseDirectiveTrivia */
//                try
//                {
//                    // Trim here to avoid needing it on each property... might remove if slow
//                    IndexValue = LogIndex.IndexDelegate(CurLine).Trim;
//                }
//                catch (Exception ex)
//                {
//                    LogCollection.LogMsg("Error getting value of index " + kvp.Key + " from " + CurLine.Id + ": " + ex.ToString(), LogLine.LogLevel.Error);
//                }
//                /* TODO ERROR: Skipped EndIfDirectiveTrivia */
//                // add to index collection if it isn't blank
//                if (IndexValue != "")
//                {
//                    mIndicies.AddLine(LogIndex.IndexName, IndexValue, CurLine);

//                    // For any index that specifies a placeholder to normalize to...
//                    if (!string.IsNullOrEmpty(LogIndex.NormalizedPlaceholder))
//                    {
//                        if (string.IsNullOrEmpty(CurLine.NormalizedByIndexMsg))
//                            CurLine.NormalizedByIndexMsg = CurLine.Message;
//                        // Replace unique index value with a normalized placeholder for a normalized message
//                        CurLine.NormalizedByIndexMsg = CurLine.NormalizedByIndexMsg.Replace(IndexValue, LogIndex.NormalizedPlaceholder);
//                    }
//                }
//            }
//        }

//        // Go through all lines to get unique messages now that all indices had a chance to normalize the line message
//        LogIndexAttribute UniqueMsgIndex = null/* TODO Change to default(_) if this is not a reference type */;
//        if (LogIndexInfo.TryGetValue(NormalizedIndexName, out UniqueMsgIndex))
//        {
//            foreach (LogLine CurLine in mLines)
//            {
//                string IndexValue = UniqueMsgIndex.IndexDelegate(CurLine).Trim;
//                if (IndexValue != "")
//                    mIndicies.AddLine(UniqueMsgIndex.IndexName, IndexValue, CurLine);
//            }
//        }

//        if (Options.Instance.UseLevMatching)
//        {
//            // collection of each "unique line" with count of how many have fuzzy matched it
//            // there must be a more performant way than using whole line as key to be hashed
//            Dictionary<string, long> UniqueLines = new Dictionary<string, long>();

//            long LineCount = 0;

//            string LevIndexName = "UniqueMessagesFuzzy";
//            foreach (LogLine CurLine in mLines)
//            {
//                if (string.IsNullOrEmpty(CurLine.NormalizedByIndexMsg))
//                    continue;

//                bool matchfound = false;

//                // skip fuzzy matching if there is an exact match
//                if (UniqueLines.ContainsKey(CurLine.NormalizedByIndexMsg))
//                {
//                    // if so, increment count for that unique line, set that unique line as index value, then continue
//                    // UniqueLines(CurLine.NormalizedByIndexMsg) += 1
//                    mIndicies.AddLine(LevIndexName, CurLine.NormalizedByIndexMsg, CurLine);
//                    matchfound = true;
//                }

//                if (!matchfound)
//                {
//                    Fastenshtein.Levenshtein lev = new Fastenshtein.Levenshtein(CurLine.NormalizedByIndexMsg);

//                    // ToArray makes a copy allowing elements to be modified without throwing collection modified error
//                    foreach (var ul in UniqueLines.Keys)
//                    {
//                        // see if current line meets fuzzy match threshold
//                        double Sim = LevSimilarity(lev, ul);

//                        if (Sim > LevThreshold)
//                        {
//                            // if so, increment count for that unique line, set that unique line as index value, then continue
//                            // UniqueLines.Item(ul) = UniqueLines.Item(ul) + 1
//                            mIndicies.AddLine(LevIndexName, ul, CurLine);
//                            CurLine.Similarity = Sim;
//                            matchfound = true;
//                            continue;
//                        }
//                    }
//                }

//                if (!matchfound)
//                {
//                    // if no match, then add as unique line and self as index value
//                    UniqueLines[CurLine.NormalizedByIndexMsg] = 1;
//                    mIndicies.AddLine(LevIndexName, CurLine.NormalizedByIndexMsg, CurLine);
//                }

//                LineCount += 1;
//                Debug.Print($"UniqueMessagesFuzzy Line {LineCount}, Unique: {UniqueLines.Count}");
//            }
//        }

//        return mIndicies;
//    }

//    private const double LevThreshold = 0.6;

//    private double LevSimilarity(Fastenshtein.Levenshtein lev, string compare)
//    {
//        return Math.Abs(1 - lev.DistanceFrom(compare) / (double)Math.Max(lev.StoredLength, compare.Length));
//    }

//    /// <summary>
//    ///     ''' Log objects are added to this collection as the log is loaded/analyzed, but then after the collections
//    ///     ''' of multiple logs are combined, this collection is replaced by the new merged collection.
//    ///     ''' Thus the lines can continue to refer to this collection and benefit from merged data from other logs.
//    ///     ''' </summary>
//    ///     ''' <value></value>
//    ///     ''' <returns></returns>
//    ///     ''' <remarks></remarks>
//    public LogObjectCollection LogObjects { get; set; } = new LogObjectCollection();

//    // Public Overridable Function IndexObjects() As Dictionary(Of String, IndexObject)

//    // Return New Dictionary(Of String, IndexObject)
//    // End Function

//    /// <summary>
//    ///     ''' Inherriting class is responsible for returning the right type of line instead of doing it here with Type.GetInstance, Activator.CreateInstance, etc
//    ///     ''' </summary>
//    ///     ''' <param name="Line"></param>
//    ///     ''' <param name="LineNumber"></param>
//    ///     ''' <returns></returns>
//    ///     ''' <remarks></remarks>
//    public virtual LogLine CreateLine(string Line, long LineNumber)
//    {
//        throw new NotImplementedException("CreateLine must be overridden if Load is not overriden for a given Log type!");
//    }

//    // TODO: remove overridable and use this only as a runtime setting on the blank instance (via menus)
//    public virtual bool TypeIsEnabled { get; set; } = true;

//    /// <summary>
//    ///     ''' Inherriting class can define criteria to decide if a file represents that type of log
//    ///     ''' </summary>
//    ///     ''' <param name="Filename"></param>
//    ///     ''' <returns>Tristate boolean where null is unsure (not yes, but not no)</returns>
//    ///     ''' <remarks></remarks>
//    public virtual bool? FileRepresentsThisType(string Filename)
//    {
//        // Assume the file doesn't represent any type unless a sub class defines criteria
//        return false;
//    }

//    /// <summary>
//    ///     ''' Create a LogFile of the provided type, either with a FileName or a blank instance
//    ///     ''' </summary>
//    ///     ''' <param name="T"></param>
//    ///     ''' <param name="FileName"></param>
//    ///     ''' <returns></returns>
//    public static LogFile CreateLogFile(Type T, string FileName = "")
//    {
//        if (string.IsNullOrEmpty(FileName))
//            return (LogFile)Activator.CreateInstance(T);
//        else
//            return (LogFile)Activator.CreateInstance(T, new[] { FileName });
//    }

//    /// <summary>
//    ///     ''' Create a blank instance of the type and (if the type is enabled) return whether the file represents the type
//    ///     ''' </summary>
//    ///     ''' <param name="T"></param>
//    ///     ''' <param name="FileName"></param>
//    ///     ''' <returns>Tristate boolean where null is unsure (not yes, but not no)</returns>
//    ///     ''' <remarks>Static function for non static (overriden) functions that can be called on a blank instance</remarks>
//    public static bool? FileRepresentsType(Type T, string FileName)
//    {
//        // don't actually pass filename to which might start parsing work depending on the type
//        LogFile l = CreateLogFile(T);
//        if (l.TypeIsEnabled)
//            // Filename is instead passed to the specific function
//            return l.FileRepresentsThisType(FileName);
//        else
//            return false;
//    }

//    public void BenchmarkLoad(string Description)
//    {
//        Stopwatch sw = new Stopwatch();
//        sw.Start();
//        Load();
//        sw.Stop();

//        FormattedBytes PrivateBytes = new FormattedBytes(Process.GetCurrentProcess().PrivateMemorySize64);

//        Console.WriteLine(Description + ": " + sw.Elapsed.ToString() + ", " + PrivateBytes.ToString() + ", Lines: " + mLines.Count);
//        if (Description != "")
//            My.Computer.FileSystem.WriteAllText(File.FullName + "-ParseBenchmark.txt", Constants.vbNewLine + Description + ": " + sw.Elapsed.ToString() + ", " + PrivateBytes.ToString() + ", Lines: " + mLines.Count, true);
//    }

//    /// <summary>
//    ///     ''' Format bytes suitably for display to user (ex: 12.3 KB, 34.41 MB).
//    ///     ''' If you need to handle sizes > Exabyte size, you'll have to modify the
//    ///     ''' class to accept ULong bytes and format accordingly.
//    ///     ''' Based on code as published here: http://forums.asp.net/t/304193.aspx
//    ///     ''' By Bart De Smet -- http://bartdesmet.net/members/bart.aspx
//    ///     ''' Algorithm fixed to handle KB correctly and modified to use Long
//    ///     ''' instead of int to handle larger file sizes.
//    ///     ''' </summary>
//    public struct FormattedBytes
//    {
//        public FormattedBytes(long size)
//        {
//            _bytes = size;
//        }

//        private long _bytes;

//        /// <summary>
//        ///         ''' Get or set bytes.
//        ///         ''' </summary>
//        public long Bytes
//        {
//            get
//            {
//                return _bytes;
//            }
//            set
//            {
//                _bytes = value;
//            }
//        }

//        public override string ToString()
//        {
//            double s = _bytes;
//            string[] format = new string[] { "{0} bytes", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB" };
//            int i = 0;
//            while (i < format.Length && s >= 1024)
//            {
//                // replace @@ w/ ampersands
//                s = System.Convert.ToDouble(Math.Truncate(100 * s / 1024)) / 100.0;
//                i += 1;
//            }

//            return string.Format(format[i], s);
//        }
//    }
//}