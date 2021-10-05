using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public interface ILogCollection<out T> where T : LogBase<LogEntry>
    {
    }
}