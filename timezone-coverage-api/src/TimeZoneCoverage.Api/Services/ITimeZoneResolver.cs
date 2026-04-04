namespace TimeZoneCoverage.Api.Services;

public interface ITimeZoneResolver
{
    TimeZoneInfo Resolve(string timeZoneId);
}
