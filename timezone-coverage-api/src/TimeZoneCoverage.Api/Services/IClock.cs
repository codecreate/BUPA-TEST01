namespace TimeZoneCoverage.Api.Services;

public interface IClock
{
    DateTime UtcNow { get; }
}
