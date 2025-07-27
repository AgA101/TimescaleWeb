using Microsoft.AspNetCore.Mvc;
using TimescaleApi.Application.Services;


[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly ValueService _valueService;

    public ResultsController(ValueService valueService)
    {
        _valueService = valueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredResults(
        [FromQuery] string fileName = null,
        [FromQuery] DateTime? minDate = null,
        [FromQuery] DateTime? maxDate = null,
        [FromQuery] double? minAvgValue = null,
        [FromQuery] double? maxAvgValue = null,
        [FromQuery] double? minAvgExecutionTime = null,
        [FromQuery] double? maxAvgExecutionTime = null)
    {
        var results = await _valueService.GetFilteredResultsAsync(fileName, minDate, maxDate, minAvgValue, maxAvgValue, minAvgExecutionTime, maxAvgExecutionTime);
        if (results == null || !results.Any())
        {
            return NotFound("No results found matching the filters.");
        }
        return Ok(results);
    }
}