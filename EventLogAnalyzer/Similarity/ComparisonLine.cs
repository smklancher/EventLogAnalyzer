using Fastenshtein;

namespace Similarity
{
    public class ComparisonLine<T> where T : notnull
    {
        // is there any use in considering string interning as part of optimization?
        // helper: https://github.com/astef/InternedString
        // for events that do actually use the exact same message, this would prevent multiple instances and make for quick comparisons
        // but if you don't know ahead of time, then maybe this has no role to play

        public ComparisonLine(T record, Func<T, string> thingToString)
        {
            ComparisonRecord = record;
            ThingToString = thingToString;

            ComparisonString = ThingToString(ComparisonRecord);

            // should
            //if (string.IsNullOrEmpty(ComparisonString))
            //{
            //    Debug.WriteLine($"null lstring message");
            //}

            Lev = new(ComparisonString);
        }

        public static bool CheckForExactMatch { get; set; } = false;
        public static double Threshold { get; set; } = 0.6;
        public T ComparisonRecord { get; init; }
        public string ComparisonString { get; }
        public Levenshtein Lev { get; init; }
        public Func<T, string> ThingToString { get; }

        public double LengthPercentageDifference(string one, string two)
        {
            var diff = Math.Abs(one.Length - two.Length);
            var per = (1 - diff / (double)one.Length) * 100;
            return per;
        }

        public bool SimilarEnough(string record, out double similarityPercentage)
        {
            var bothBlank = string.IsNullOrWhiteSpace(ComparisonString) && string.IsNullOrWhiteSpace(record);
            if (bothBlank || (CheckForExactMatch && ComparisonString == record))
            {
                similarityPercentage = 1.0;
                return true;
            }

            var lengthPercentageDifference = LengthPercentageDifference(ComparisonString, record);

            if (lengthPercentageDifference < SimilarityOptions.Instance.MaxPercentLengthDifferenceToCompare)
            {
                similarityPercentage = 0.0;
                return false;
            }

            similarityPercentage = LevPercent(record ?? string.Empty);
            if (similarityPercentage > Threshold)
            {
                return true;
            }

            return false;
        }

        public bool SimilarEnough(T record, out double similarityPercentage)
        {
            return SimilarEnough(ThingToString(record), out similarityPercentage);
        }

        public bool SimilarEnough(ComparisonLine<T> other, out double similarityPercentage)
        {
            return SimilarEnough(other.ComparisonString, out similarityPercentage);
        }

        private double LevPercent(string compare)
        {
            return Math.Abs(1 - (double)Lev.DistanceFrom(compare) / (double)Math.Max(Lev.StoredLength, compare.Length));
        }
    }
}