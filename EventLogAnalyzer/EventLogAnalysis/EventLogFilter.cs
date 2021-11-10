namespace EventLogAnalysis
{
    internal class EventLogFilter
    {
        public static string ByProviders(IEnumerable<string> providers) => $"*[System[Provider[@Name='{string.Join("' or @Name='", providers)}']]]";
    }
}