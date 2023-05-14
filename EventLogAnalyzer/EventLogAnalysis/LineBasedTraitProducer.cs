using System.Text.RegularExpressions;

namespace EventLogAnalysis;

public class LineBasedTraitProducer
{
    public void AddTraitsFromLine(TraitTypeCollection traits, ELRecord r)
    {
        r.AddTrait("Provider", r.ProviderName, traits);
        r.AddTrait("Level", r.Level, traits);

        if (r.MessageLoadExeption is not null)
        {
            r.AddTrait("EventLogReadingException", r.MessageLoadExeption.Message, traits);
        }

        if (r.Message.Contains("App Domain:"))
        {
            var ellt = new EnterpriseLibraryLogText(r.Message);

            // for short message use inner exception or StackTraceTopFrame if available, preferring StackTraceTopFrame

            if (!string.IsNullOrWhiteSpace(ellt.InnerException))
            {
                r.ShortMessage = ellt.InnerException;
                r.AddTrait("Inner Exception", ellt.InnerException, traits);
            }

            if (!string.IsNullOrWhiteSpace(ellt.StackTraceTopFrame))
            {
                r.ShortMessage = ellt.StackTraceTopFrame;
                r.AddTrait("StackTraceTopFrame", ellt.StackTraceTopFrame, traits);
            }

            if (!string.IsNullOrWhiteSpace(ellt.StackTraceBottomFrame))
            {
                r.AddTrait("StackTraceBottomFrame", ellt.StackTraceBottomFrame, traits);
            }

            if (!string.IsNullOrWhiteSpace(ellt.RequestUrl))
            {
                r.AddTrait("RequestUrl", ellt.RequestUrl, traits);
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
                    r.AddTrait("Faulting Application", name, traits);
                }
                if (!string.IsNullOrWhiteSpace(excode))
                {
                    r.AddTrait("Faulting Exception Code", excode, traits);
                }
            }
        }
    }
}