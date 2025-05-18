namespace Warden.Bot.Extensions;

public static class DateTimeExtensions
{
    /// Convert datetime to UNIX time
    public static string ToUnixTime(this DateTime dateTime)
    {
        DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
        return dto.ToUnixTimeSeconds().ToString();
    }
 
    /// Convert datetime to UNIX time including miliseconds
    public static string ToUnixTimeMilliSeconds(this DateTime dateTime)
    {
        DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
        return dto.ToUnixTimeMilliseconds().ToString();
    }
}