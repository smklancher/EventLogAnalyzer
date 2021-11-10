namespace Similarity
{
    public class WorkingSetGroup<T> where T : notnull
    {
        public List<SimilarLineGroup<T>> SubGroups = new();

        public WorkingSetGroup(IEnumerable<T> input, Func<T, string> thingToString)
        {
            InputSet = input;
            ThingToString = thingToString;
        }

        public Func<T, string> ThingToString { get; }
        //public string GroupKey { get; init; }
        //public List<T> Records { get; private set; } = new();

        private IEnumerable<T> InputSet { get; }

        public void GroupSimilarLines()
        {
            foreach (var r in InputSet)
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
                    var slg = new SimilarLineGroup<T>(r, ThingToString);
                    SubGroups.Add(slg);
                }
            }

            //Debug.WriteLine($"{GroupKey} has {Records.Count} records over {SubGroups.Count} subgroups");
        }

        public void Merge(WorkingSetGroup<T> other)
        {
            foreach (var os in other.SubGroups)
            {
                bool mergedInToGroup = false;

                foreach (var s in SubGroups)
                {
                    // if the comparison lines between the groups are similar enough
                    if (s.ComparisonLine.SimilarEnough(os.ComparisonLine, out double similarityPercentage))
                    {
                        // then merge the other group into this one
                        s.Merge(os);
                        mergedInToGroup = true;
                    }
                }

                if (!mergedInToGroup)
                {
                    // it it wasn't merged into any groups, then add it
                    SubGroups.Add(os);
                }
            }
        }
    }
}