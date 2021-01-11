using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class SimilarLineGroup : SingleTraitValueEventCollection
    {
        public SimilarLineGroup(ELRecord record)
        {
            ComparisonLine = new(record);
            TraitName = "SimilarLines";
            TraitValue = ComparisonLine.TraitValue;

            // add itself
            record.GroupSimilarity = 1;
            Lines.Add(record);
        }

        public ComparisonLine ComparisonLine { get; init; }

        public bool AddIfSimilar(ELRecord record)
        {
            if (ComparisonLine.SimilarEnough(record))
            {
                Lines.Add(record);
                return true;
            }

            return false;
        }
    }
}