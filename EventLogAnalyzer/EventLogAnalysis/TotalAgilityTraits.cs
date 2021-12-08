using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class TotalAgilityTraits
    {
        public static void TotalAgilityTenantName(TraitTypeCollection traits, ELRecord r)
        {
            // language=regex
            string msgregex = @"\w+_(?<dep>live|dev)";

            var ContentMatch = Regex.Match(r.Message, msgregex, RegexOptions.Singleline);
            if (ContentMatch.Success)
            {
                var dep = ContentMatch.Groups["dep"].Value.Trim();
                var tenantcode = ContentMatch.Value;

                if (!string.IsNullOrWhiteSpace(dep))
                {
                    traits.AddLine("TotalAgility Deployment Type", dep, r);
                }
                if (!string.IsNullOrWhiteSpace(tenantcode))
                {
                    traits.AddLine("TotalAgility Tenant Code", tenantcode, r);
                }
            }

            // language=regex
            string formtwo = @"Tenant Code\s-\s(?<name>\w+)";

            var ContentMatch2 = Regex.Match(r.Message, formtwo, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (ContentMatch2.Success)
            {
                var tenantcode = ContentMatch2.Groups["name"].Value.Trim();

                if (!string.IsNullOrWhiteSpace(tenantcode))
                {
                    traits.AddLine("TotalAgility Tenant Code", tenantcode, r);
                }
            }
        }
    }
}