using TimeZoneCoverage.Api.Models;
using TimeZoneCoverage.Api.Services;
using Xunit;

namespace TimeZoneCoverage.Api.Tests;

public sealed class CoverageActivationServiceTests
{
    private readonly ICoverageActivationService _service = new CoverageActivationService(new TimeZoneResolver());

    [Fact]
    public void ScheduleActivation_SydneyStandardTime_ConvertsToCorrectUtc()
    {
        var request = new CoverageScheduleRequest
        {
            CustomerId = "cust-001",
            SelectedStartDate = new DateOnly(2026, 5, 1),
            PurchaseTimeZoneId = "Asia/Kolkata",
            ActivationTimeZoneId = "Australia/Sydney"
        };

        var response = _service.ScheduleActivation(request);

        // On 2026-05-01, Sydney is UTC+10, so local midnight is 2026-04-30T14:00:00Z.
        Assert.Equal(new DateTime(2026, 4, 30, 14, 0, 0, DateTimeKind.Utc), response.ActivationUtc);
    }

    [Fact]
    public void ScheduleActivation_ShouldRespectDstForSydneyInJanuary()
    {
        var request = new CoverageScheduleRequest
        {
            CustomerId = "cust-002",
            SelectedStartDate = new DateOnly(2026, 1, 15),
            PurchaseTimeZoneId = "Asia/Kolkata",
            ActivationTimeZoneId = "Australia/Sydney"
        };

        var response = _service.ScheduleActivation(request);

        // January in Sydney is daylight saving time (UTC+11).
        // Local midnight 2026-01-15 in Sydney should be 2026-01-14T13:00:00Z.
        Assert.Equal(new DateTime(2026, 1, 14, 13, 0, 0, DateTimeKind.Utc), response.ActivationUtc);
    }

    [Fact]
    public void CoverageStatus_ShouldBeInactive_BeforeActivationUtc()
    {
        var status = _service.GetCoverageStatus(new CoverageStatusRequest
        {
            ActivationUtc = new DateTime(2026, 4, 30, 14, 0, 0, DateTimeKind.Utc),
            CurrentUtc = new DateTime(2026, 4, 30, 13, 59, 59, DateTimeKind.Utc)
        });

        Assert.False(status.IsActive);
    }
}
