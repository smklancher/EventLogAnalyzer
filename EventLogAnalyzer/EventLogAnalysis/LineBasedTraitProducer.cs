using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class LineBasedTraitProducer
    {
        // language=regex
        private const string msgregex = "Timestamp:(?<timestamp>.+)Message:(?<message>.+)Category:(?<category>.+)Priority:(?<priority>.+)EventId:(?<eventid>.+)" +
            "Severity:(?<severity>.+)Title:(?<title>.+)Machine:(?<machine>.+)App Domain:(?<appdomain>.+)ProcessId:(?<processid>.+)Process Name:(?<processname>.+)Thread Name:(?<threadname>.+)" +
            "Win32 ThreadId:(?<threadid>.+)Extended Properties:(?<extentedproperties>.+)";

        public void AddTraitsFromLine(TraitTypeCollection traits, ELRecord r)
        {
            traits.AddLine("Provider", r.ProviderName, r);
            if (r.MessageLoadExeption is not null)
            {
                traits.AddLine("EventLogReadingException", r.MessageLoadExeption.Message, r);
            }

            if (r.Message.Contains("App Domain:"))
            {
                var ellt = new EnterpriseLibraryLogText(r.Message);
                r.ShortMessage = ellt.OuterException;

                if (!string.IsNullOrWhiteSpace(ellt.InnerException))
                {
                    traits.AddLine("Inner Exception", ellt.InnerException, r);
                }
            }

            CrashMessageTraits(traits, r);
        }

        private static void CrashMessageTraits(TraitTypeCollection traits, ELRecord r)
        {
            if (r.Message.Contains("Faulting application name: "))
            {
                // language=regex
                string msgregex = @"Faulting application name: (?<name>.+?)\.exe(.+?)Exception code: (?<excode>0x[0-9A-Fa-f]{8})";

                var ContentMatch = Regex.Match(r.Message, msgregex, RegexOptions.Singleline);
                if (ContentMatch.Success)
                {
                    var name = ContentMatch.Groups["name"].Value.Trim();
                    var excode = ContentMatch.Groups["excode"].Value.Trim();

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        traits.AddLine("Faulting Application", name, r);
                    }
                    if (!string.IsNullOrWhiteSpace(excode))
                    {
                        traits.AddLine("Faulting Exception Code", excode, r);
                    }
                }
            }
        }
    }
}