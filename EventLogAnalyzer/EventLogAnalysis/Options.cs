using System.ComponentModel;
using Similarity;

namespace EventLogAnalysis;

public class Options
{
    private const string SimilarityCategory = "Similarity";
    private const string TestingCategory = "Testing";
    private const string TimestampCategory = "Timestamp";

    private static readonly Lazy<Options> Lazy =
                                new Lazy<Options>(() => new Options());

    private Options()
    {
    }

    public static Options Instance => Lazy.Value;

    [Description("Enable whatever current behavior is being tested during development.")]
    [Category(TestingCategory)]
    public bool EnableTextLogTest { get; set; } = false;

    [Category(TimestampCategory)]
    public int HourOffset { get; set; } = 0;

    [Description("Include filters use OR, meaning find messages that match this or that.  Set false for AND search.  Exclude is always OR, meaning exlude if message matches this or that.")]
    [Category(TestingCategory)]
    public bool IncludeFilterUsesOR { get; set; } = true;

    [Description("If an MTA file is present in the LocaleMetaData subfolder then the EventLog APIs perform wildly slower.  This renames the file to avoid the performance penalty while loading then renames it back.")]
    [Category(TestingCategory)]
    public bool RenameMtaDuringLoad { get; set; } = true;

    [Category(SimilarityCategory)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public SimilarityOptions SimilarityOptions { get; set; } = Similarity.SimilarityOptions.Instance;

    [Description("Just combine event properties instead of requesting formated event message")]
    [Category(TestingCategory)]
    public bool SkipFormattedMessage { get; set; } = false;

    [Category(TimestampCategory)]
    public OffsetOption TimestampConversion { get; set; } = OffsetOption.ConvertToLocal;

    [Description("Show columns for earliest and last date before actual value.  Not wired to be changed at runtime.")]
    [Category(TestingCategory)]
    public bool TraitDatesBeforeTraitValue { get; set; } = true;

    public void OnCloseOptionsForm()
    {
    }
}