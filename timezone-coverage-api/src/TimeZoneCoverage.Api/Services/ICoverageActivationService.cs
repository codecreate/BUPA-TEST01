using TimeZoneCoverage.Api.Models;

namespace TimeZoneCoverage.Api.Services;

public interface ICoverageActivationService
{
    CoverageScheduleResponse ScheduleActivation(CoverageScheduleRequest request);
    CoverageStatusResponse GetCoverageStatus(CoverageStatusRequest request);
}
