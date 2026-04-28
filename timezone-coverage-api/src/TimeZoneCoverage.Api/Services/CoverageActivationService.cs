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

        // changed the name of the function being called here. It was previously named ConvertStartDateToActivationUtc_Buggy
        var activationUtc = ConvertStartDateToActivationUtc(request.SelectedStartDate, activationTimeZone);

        var activationLocal = TimeZoneInfo.ConvertTimeFromUtc(activationUtc, activationTimeZone);
        // DateTimeOffset is a DateTime plus an explicit offset. By constructing one with the activation timezone's
        // correct offset attached, zzz now has the right answer to give. It's baked in, not guessed from the server environment.
        var activationLocalOffset = new DateTimeOffset(activationLocal, activationTimeZone.GetUtcOffset(activationUtc));

        return new CoverageScheduleResponse
        {
            CustomerId = request.CustomerId,
            SelectedStartDate = request.SelectedStartDate,
            PurchaseTimeZoneId = request.PurchaseTimeZoneId,
            ActivationTimeZoneId = request.ActivationTimeZoneId,
            ActivationUtc = activationUtc,
            ActivationLocalTime = activationLocalOffset.ToString("yyyy-MM-dd HH:mm:ss zzz"),
            Notes = string.Empty
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

    // CHANGED TO SATISFY THE CRITERIA: The original method (ConvertStartDateToActivationUtc_Buggy) called DateTime.SpecifyKind(..., DateTimeKind.Utc),
    // which incorrectly treated the customer's selected date as already being midnight UTC. That produced the wrong
    // activation time for any customer whose activation timezone differs from UTC, and ignored DST entirely.
    //
    // This implementation constructs midnight with DateTimeKind.Unspecified — meaning "midnight as the clock reads in that timezone"
    // — and passes it to TimeZoneInfo.ConvertTimeToUtc, which applies the correct UTC offset including any DST adjustment.
    // Example: 2026-05-01 midnight in Australia/Sydney (UTC+10) correctly becomes 2026-04-30T14:00:00Z.
    private static DateTime ConvertStartDateToActivationUtc(DateOnly selectedStartDate, TimeZoneInfo activationTimeZone)
    {
        var localMidnight = selectedStartDate.ToDateTime(TimeOnly.MinValue); // Kind == Unspecified: no tz context; ConvertTimeToUtc interprets it as being in activationTimeZone
        return TimeZoneInfo.ConvertTimeToUtc(localMidnight, activationTimeZone);
    }
}
