using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Similarity;

namespace EventLogAnalysis
{
    public class Options
    {
        private const string SimilarityCategory = "Similarity";
        private const string TestingCategory = "Testing";

        private static readonly Lazy<Options> Lazy =
                                    new Lazy<Options>(() => new Options());

        private Options()
        {
        }

        public static Options Instance => Lazy.Value;

        [Category(TestingCategory)]
        public bool RenameMtaDuringLoad { get; set; } = true;

        [Category(SimilarityCategory)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SimilarityOptions SimilarityOptions { get; set; } = Similarity.SimilarityOptions.Instance;

        [Category(TestingCategory)]
        public bool UseNewSimilarity { get; set; } = true;

        public void OnCloseOptionsForm()
        {
        }
    }
}