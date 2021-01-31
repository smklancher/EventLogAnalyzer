using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Category(SimilarityCategory)]
        public int LinesPerSimilarityGroupChunk { get; set; } = 200;

        [Category(TestingCategory)]
        public bool UseNewSimilarity { get; set; } = true;

        //[Category(SystemsCategory)]
        //[TypeConverter(typeof(ExpandableObjectConverter))]
        //public SupportCaseDownloader CaseDownloaderSystem { get; set; }

        public void OnCloseOptionsForm()
        {
        }
    }
}