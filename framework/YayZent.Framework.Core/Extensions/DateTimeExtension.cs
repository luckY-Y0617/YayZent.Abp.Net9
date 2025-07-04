namespace YayZent.Framework.Core.Extensions;

public static class DateTimeExtension
{
    public static DateTime GetWeekStart(this DateTime dateTime)
    {
        var now = DateTime.Now;
        int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
        return now.AddDays(-1 * diff).Date;
    }
}