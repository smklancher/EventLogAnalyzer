using System.Text.RegularExpressions;

namespace EventLogAnalysis;

public class LineBasedTraitProducer
{
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

            // for short message use inner exception or StackTraceTopFrame if available, preferring StackTraceTopFrame

            if (!string.IsNullOrWhiteSpace(ellt.InnerException))
            {
                r.ShortMessage = ellt.InnerException;
                traits.AddLine("Inner Exception", ellt.InnerException, r);
            }

            if (!string.IsNullOrWhiteSpace(ellt.StackTraceTopFrame))
            {
                r.ShortMessage = ellt.StackTraceTopFrame;
                traits.AddLine("StackTraceTopFrame", ellt.StackTraceTopFrame, r);
            }

            if (!string.IsNullOrWhiteSpace(ellt.StackTraceBottomFrame))
            {
                traits.AddLine("StackTraceBottomFrame", ellt.StackTraceBottomFrame, r);
            }
        }

        TotalAgilityTraits.TotalAgilityTenantName(traits, r);

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