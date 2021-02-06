using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Similarity
{
    public class SimilarityOptions
    {
        private const string SimilarityCategory = "Similarity";

        private static readonly Lazy<SimilarityOptions> Lazy =
                                    new Lazy<SimilarityOptions>(() => new SimilarityOptions());

        private SimilarityOptions()
        {
        }

        public static SimilarityOptions Instance => Lazy.Value;

        [Category(SimilarityCategory)]
        public int LinesPerSimilarityGroupChunk { get; set; } = 200;

        public int MaxPercentLengthDifferenceToCompare { get; set; } = 90;
    }
}