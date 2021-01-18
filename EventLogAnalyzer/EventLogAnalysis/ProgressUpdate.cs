using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public record ProgressUpdate(bool RefreshUI, string StatusBarText);
}