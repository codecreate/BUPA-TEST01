namespace TimeZoneCoverage.Api.Models;

public sealed class CoverageStatusResponse
{
    public bool IsActive { get; set; }
    public string Reason { get; set; } = string.Empty;
}
