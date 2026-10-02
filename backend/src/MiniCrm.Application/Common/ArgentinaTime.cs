namespace MiniCrm.Application.Common;

public static class ArgentinaTime
{
    public static readonly TimeZoneInfo Zone = Resolve();

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone);

    public static DateTime Today => Now.Date;

    public static TimeSpan OffsetFor(DateTime dateTime) => Zone.GetUtcOffset(dateTime);

    private static TimeZoneInfo Resolve()
    {
        foreach (var id in new[] { "Argentina Standard Time", "America/Argentina/Buenos_Aires" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.CreateCustomTimeZone(
            "ART",
            TimeSpan.FromHours(-3),
            "Argentina (UTC-3)",
            "Argentina (UTC-3)");
    }
}
