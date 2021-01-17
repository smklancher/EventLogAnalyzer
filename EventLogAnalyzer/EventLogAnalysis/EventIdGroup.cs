using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityCommon;

namespace EventLogAnalysis
{
    public class EventIdGroup
    {
        public List<SimilarLineGroup> SubGroups = new();

        public EventIdGroup(ProviderEventIdPair groupKey)
        {
            GroupKey = groupKey;
        }

        public ProviderEventIdPair GroupKey { get; init; }
        public List<ELRecord> Records { get; private set; } = new();

        public void GroupSimilarLines()
        {
            using var dt = new DisposableTrace(Label: GroupKey.ToString());
            foreach (var r in Records)
            {
                bool addedToGroup = false;

                foreach (var s in SubGroups)
                {
                    if (s.AddIfSimilar(r, out _))
                    {
                        addedToGroup = true;
                        break;
                    }
                }

                // did not meet any existing group so add a new one
                if (!addedToGroup)
                {
                    var slg = new SimilarLineGroup(r);
                    SubGroups.Add(slg);
                }
            }

            Debug.WriteLine($"{GroupKey} has {Records.Count} records over {SubGroups.Count} subgroups");
        }
    }
}