namespace Similarity
{
    public class SimilarLineGroup<T> where T : notnull
    {
        public SimilarLineGroup(T record, Func<T, string> thingToString)
        {
            ThingToString = thingToString;
            ComparisonLine = new(record, thingToString);

            // add itself
            //record.GroupSimilarity = 1;
            Items.Add(record);
        }

        public ComparisonLine<T> ComparisonLine { get; init; }
        public List<T> Items { get; } = new();
        public Func<T, string> ThingToString { get; }

        public bool AddIfSimilar(T record, out double similarityPercentage)
        {
            if (ComparisonLine.SimilarEnough(record, out similarityPercentage))
            {
                Items.Add(record);
                return true;
            }

            return false;
        }

        public void Merge(SimilarLineGroup<T> other)
        {
            Items.AddRange(other.Items);
        }
    }
}