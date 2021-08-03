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

        [Description("Lines are split into groups of this size so similarity calc can be parallelized.")]
        [Category(SimilarityCategory)]
        public int LinesPerSimilarityGroupChunk { get; set; } = 200;

        [Description("If the shorter message is this percentage length less, then they are assumed not to be similar, thus quicker than doing a lev distance calculation.")]
        public int MaxPercentLengthDifferenceToCompare { get; set; } = 90;
    }
}