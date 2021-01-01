using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public interface ILogLineDisplay
    {
        string LevelString { get; }
        string Message { get; }
        string TimestampString { get; }
    }
}