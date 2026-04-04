namespace TimeZoneCoverage.Api.Models;

public sealed class CoverageScheduleRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public DateOnly SelectedStartDate { get; set; }
    public string PurchaseTimeZoneId { get; set; } = string.Empty;
    public string ActivationTimeZoneId { get; set; } = string.Empty;
}
