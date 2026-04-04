using TimeZoneCoverage.Api.Models;

namespace TimeZoneCoverage.Api.Services;

public sealed class CoverageActivationService : ICoverageActivationService
{
    private readonly ITimeZoneResolver _timeZoneResolver;

    public CoverageActivationService(ITimeZoneResolver timeZoneResolver)
    {
        _timeZoneResolver = timeZoneResolver;
    }

    public CoverageScheduleResponse ScheduleActivation(CoverageScheduleRequest request)
    {
        var activationTimeZone = _timeZoneResolver.Resolve(request.ActivationTimeZoneId);

        // Intentionally incorrect starter implementation.
        // It assumes the selected date is midnight UTC, which breaks local activation
        // and DST scenarios when the policy should start at midnight in the activation time zone.
        var activationUtc = ConvertStartDateToActivationUtc_Buggy(request.SelectedStartDate, activationTimeZone);

        var activationLocal = TimeZoneInfo.ConvertTimeFromUtc(activationUtc, activationTimeZone);

        return new CoverageScheduleResponse
        {
            CustomerId = request.CustomerId,
            SelectedStartDate = request.SelectedStartDate,
            PurchaseTimeZoneId = request.PurchaseTimeZoneId,
            ActivationTimeZoneId = request.ActivationTimeZoneId,
            ActivationUtc = activationUtc,
            ActivationLocalTime = activationLocal.ToString("yyyy-MM-dd HH:mm:ss zzz"),
            Notes = "Current implementation is intentionally incorrect. Fix activation so it starts at local midnight in the target time zone, including DST-aware conversion."
        };
    }

    public CoverageStatusResponse GetCoverageStatus(CoverageStatusRequest request)
    {
        var isActive = request.CurrentUtc >= request.ActivationUtc;

        return new CoverageStatusResponse
        {
            IsActive = isActive,
            Reason = isActive
                ? "Coverage is active."
                : "Coverage has not started yet."
        };
    }

    private static DateTime ConvertStartDateToActivationUtc_Buggy(DateOnly selectedStartDate, TimeZoneInfo activationTimeZone)
    {
        var midnight = selectedStartDate.ToDateTime(TimeOnly.MinValue);

        // BUG: this assumes the date selected by the customer is already UTC.
        // For Australia/Sydney on 2026-05-01, this returns 2026-05-01T00:00:00Z,
        // but correct activation should be local midnight in Sydney converted to UTC.
        return DateTime.SpecifyKind(midnight, DateTimeKind.Utc);
    }
}
