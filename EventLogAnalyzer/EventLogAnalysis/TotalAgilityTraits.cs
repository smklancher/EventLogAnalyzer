using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class TotalAgilityTraits
    {
        public static void TotalAgilityTenantName(TraitTypeCollection traits, ELRecord r)
        {
            //splitting into separate functions to look into performance of each separately
            // TenantNameForm1(traits, r); poor performing regex, disabled for now
            TenantNameForm2(traits, r);
        }

        private static void TenantNameForm1(TraitTypeCollection traits, ELRecord r)
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
                    r.AddTrait("TotalAgility Deployment Type", dep, traits);
                }
                if (!string.IsNullOrWhiteSpace(tenantcode))
                {
                    r.AddTrait("TotalAgility Tenant Code", tenantcode, traits);
                }
            }
        }

        private static void TenantNameForm2(TraitTypeCollection traits, ELRecord r)
        {
            // language=regex
            string formtwo = @"Tenant Code\s-\s(?<name>\w+)";

            var ContentMatch2 = Regex.Match(r.Message, formtwo, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (ContentMatch2.Success)
            {
                var tenantcode = ContentMatch2.Groups["name"].Value.Trim();

                if (!string.IsNullOrWhiteSpace(tenantcode))
                {
                    r.AddTrait("TotalAgility Tenant Code", tenantcode, traits);
                }
            }
        }
    }
}