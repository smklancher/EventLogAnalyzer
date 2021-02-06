using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class EventTextProperties
    {
        public EventTextProperties(string eventText)
        {
            EventText = eventText;
        }

        public string EventText { get; }
    }
}