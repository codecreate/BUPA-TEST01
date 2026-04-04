namespace TimeZoneCoverage.Api.Models;

public sealed class CoverageStatusRequest
{
    public DateTime ActivationUtc { get; set; }
    public DateTime CurrentUtc { get; set; }
}
