using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class ProgressUpdate
    {
        public ProgressUpdate()
        {
        }

        public ProgressUpdate(bool refresh, string statusText)
        {
            RefreshUI = refresh;
            StatusBarUpdate = statusText;
        }

        public bool RefreshLogsView { get; set; }
        public bool RefreshUI { get; set; }
        public string StatusBarUpdate { get; set; } = string.Empty;
    }
}