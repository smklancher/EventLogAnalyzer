using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Similarity;

namespace EventLogAnalysis
{
    public static class Converter
    {
        public const string SimilarLinesString = "SimilarLines";

        public static SingleTraitValueEventCollection FromSimilarLineGroupToSingleTraitValue(Similarity.SimilarLineGroup<ELRecord> similarLineGroup)
        {
            var stv = new SingleTraitValueEventCollection
            {
                TraitName = SimilarLinesString,
                TraitValue = similarLineGroup.ComparisonLine.ComparisonString
            };

            foreach (var sl in similarLineGroup.Items)
            {
                stv.Add(sl);
            }

            return stv;
        }

        public static TraitValuesCollection FromWorkingSetGroupToTraitValuesCollection(Similarity.WorkingSetGroup<ELRecord> workingSetGroup)
        {
            var tvc = new TraitValuesCollection(SimilarLinesString);

            foreach (var slg in workingSetGroup.SubGroups)
            {
                var stv = FromSimilarLineGroupToSingleTraitValue(slg);
                tvc.AddCollection(stv);
            }

            return tvc;
        }
    }
}