using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using Serilog;

namespace Similarity
{
    public class Processing
    {
        //SimilarLineGroup to SingleTraitValueEventCollection

        //input group of strings, or objects with delegate to return a string
        //input similarity criteria.  Can make more complex later, but simple is % from lev distance
        //input either designer parallelism, or more likely max working set size (in which each in the set must be compared to any groups in the set)
        //maybe input options or delegates for string preprocessing (replace dates, guids?)

        public static WorkingSetGroup<T> MergeWorkingSets<T>(IEnumerable<WorkingSetGroup<T>> workingSets) where T : notnull
        {
            if (!workingSets.Any()) { throw new ArgumentOutOfRangeException("Cannot merge 0 working set groups"); }

            var stopwatch = Stopwatch.StartNew();
            var merged = workingSets.First();
            foreach (var w in workingSets.Skip(1))
            {
                merged.Merge(w);
            }

            Log.Information($"Merged similarity groups ({stopwatch.ElapsedMilliseconds:n0} ms)");

            return merged;
        }

        public static WorkingSetGroup<T> Process<T>(IEnumerable<T> things, int chunkSize, Func<T, string> thingToString) where T : notnull
        {
            if (!things.Any()) { return new WorkingSetGroup<T>(things, thingToString); }

            // split up into chunks for parallel procesisng
            var chunks = things.Batch(chunkSize, x => new WorkingSetGroup<T>(x, thingToString)).ToList();
            if (chunks is null)
            {
                throw new ArgumentNullException("batch");
            }

            return ProcessWorkingSets(chunks);
        }

        private static WorkingSetGroup<T> ProcessWorkingSets<T>(IEnumerable<WorkingSetGroup<T>> workingSets) where T : notnull
        {
            // might be interesting to try using threading channels for this:
            // https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/

            var stopwatch = Stopwatch.StartNew();
            var opts = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 1, // give the system a little bit of room for other things
                //TODO: cancelation
            };

            // for now simply run the work in parallel
            Parallel.ForEach(workingSets, opts, x => x.GroupSimilarLines());
            Log.Information($"Finished parallel processing of {workingSets.Count()} similarity groups ({stopwatch.ElapsedMilliseconds:n0} ms)");

            stopwatch.Restart();

            // merged all working sets into the first one, then return that
            var merged = MergeWorkingSets(workingSets);

            return merged;
        }
    }
}