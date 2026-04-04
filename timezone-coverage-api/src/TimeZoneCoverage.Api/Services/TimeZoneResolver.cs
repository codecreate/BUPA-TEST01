namespace TimeZoneCoverage.Api.Services;

public sealed class TimeZoneResolver : ITimeZoneResolver
{
    public TimeZoneInfo Resolve(string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            throw new ArgumentException("Time zone ID is required.", nameof(timeZoneId));
        }

        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }
}
