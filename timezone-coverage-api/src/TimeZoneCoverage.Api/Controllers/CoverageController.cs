using Microsoft.AspNetCore.Mvc;
using TimeZoneCoverage.Api.Models;
using TimeZoneCoverage.Api.Services;

namespace TimeZoneCoverage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CoverageController : ControllerBase
{
    private readonly ICoverageActivationService _coverageActivationService;

    public CoverageController(ICoverageActivationService coverageActivationService)
    {
        _coverageActivationService = coverageActivationService;
    }

    [HttpPost("schedule")]
    [ProducesResponseType(typeof(CoverageScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CoverageScheduleResponse> Schedule([FromBody] CoverageScheduleRequest request)
    {
        if (request.SelectedStartDate == default)
        {
            return BadRequest("SelectedStartDate is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ActivationTimeZoneId))
        {
            return BadRequest("ActivationTimeZoneId is required.");
        }

        var response = _coverageActivationService.ScheduleActivation(request);
        return Ok(response);
    }

    [HttpPost("status")]
    [ProducesResponseType(typeof(CoverageStatusResponse), StatusCodes.Status200OK)]
    public ActionResult<CoverageStatusResponse> Status([FromBody] CoverageStatusRequest request)
    {
        var response = _coverageActivationService.GetCoverageStatus(request);
        return Ok(response);
    }
}
