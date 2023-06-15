namespace QuoteMonitor;

public static class Util
{
    public static long FromDateToTimestamp(string date)
    {
        return DateTimeOffset.Parse(date).ToUnixTimeMilliseconds();
    }

    public static string FromTimestampToDate(long timestamp)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
        DateTime date = dateTimeOffset.DateTime;
        return date.ToString("yyyy-MM-dd HH:mm:ss");
    }
}