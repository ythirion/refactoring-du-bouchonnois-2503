namespace Bouchonnois.Tests.Acceptances;

public static class TimeExtensions
{
    public static TimeSpan HoursLater(this int hours) => TimeSpan.FromHours(hours);
    public static TimeSpan MinutesLater(this int minutes) => TimeSpan.FromMinutes(minutes);
    public static TimeSpan SecondsLater(this int seconds) => TimeSpan.FromSeconds(seconds);
}