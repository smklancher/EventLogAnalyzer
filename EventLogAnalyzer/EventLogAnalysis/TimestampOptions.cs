namespace EventLogAnalysis
{
    public enum OffsetOption
    {
        ConvertToLocal,
        UTC,
        OffsetFromLocal,
        OffsetFromUTC
    }

    public class TimestampOptions
    {
        public static DateTime Convert(DateTime dateTime)
        {
            // event log returns timestamp as local even though internally is UTC
            if (Options.Instance.TimestampConversion == OffsetOption.ConvertToLocal) { return dateTime; }

            var localDate = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            var newDate = Options.Instance.TimestampConversion switch
            {
                OffsetOption.UTC => localDate.ToUniversalTime(),
                OffsetOption.OffsetFromLocal => dateTime.AddHours(Options.Instance.HourOffset),
                OffsetOption.OffsetFromUTC => localDate.ToUniversalTime().AddHours(Options.Instance.HourOffset),
                OffsetOption.ConvertToLocal => dateTime,
                _ => dateTime,
            };

            return newDate;
        }

        public static string ConvertToString(DateTime? dateTime)
        {
            if (dateTime is null)
            {
                return string.Empty;
            }
            else
            {
                return Convert((DateTime)dateTime).ToString();
            }
        }
    }
}