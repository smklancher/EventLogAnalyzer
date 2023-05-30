using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace EventLogAnalyzer
{
    internal class LineSource
    {
        private static readonly Lazy<LineSource> Lazy =
                                    new Lazy<LineSource>(() => new LineSource());

        private LineSource()
        {
        }

        public static LineSource Instance => Lazy.Value;

        public Func<ILogEntryCollection<LogEntry>> Func { get; set; } = () => new LogEntryCollection<LogEntry>();

        public static ILogEntryCollection<LogEntry> LinesFromSource() => Instance.Func();

        public static void SetSource(Func<ILogEntryCollection<LogEntry>> func)
        {
            Instance.Func = func;
        }

        //IsDisplayingFullFile ? FileList.SelectedLogEvents : Logs.TraitTypes.Lines(TraitTypesList.SelectedTraitType(), TraitValuesList.ActiveTraitValue);
    }
}