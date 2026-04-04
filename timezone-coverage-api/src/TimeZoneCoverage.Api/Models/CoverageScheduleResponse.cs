namespace TimeZoneCoverage.Api.Models;

public sealed class CoverageScheduleResponse
{
    public string CustomerId { get; set; } = string.Empty;
    public DateOnly SelectedStartDate { get; set; }
    public string PurchaseTimeZoneId { get; set; } = string.Empty;
    public string ActivationTimeZoneId { get; set; } = string.Empty;
    public DateTime ActivationUtc { get; set; }
    public string ActivationLocalTime { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
