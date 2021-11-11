namespace EventLogAnalysis;

public static class SimilarityConverter
{
    public const string SimilarLinesString = "SimilarLines";

    public static SingleTraitValueEventCollection FromSimilarLineGroupToSingleTraitValue(Similarity.SimilarLineGroup<LogEntry> similarLineGroup)
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

    public static TraitValuesCollection FromWorkingSetGroupToTraitValuesCollection(Similarity.WorkingSetGroup<LogEntry> workingSetGroup)
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