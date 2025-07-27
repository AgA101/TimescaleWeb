using Microsoft.AspNetCore.Mvc;
using TimescaleApi.Application.Services;

namespace TimescaleApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly ValueService _valueService;

        public ValuesController(ValueService valueService)
        {
            _valueService = valueService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _valueService.ProcessCsvAsync(stream, file.FileName);
                return Ok(new { Message = "File processed successfully", Result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{fileName}/last10")]
        public async Task<IActionResult> GetLast10Values(string fileName)
        {
            var values = await _valueService.GetLast10ValuesAsync(fileName);
            if (values == null || !values.Any())
            {
                return NotFound($"No values found for fileName: {fileName}");
            }
            return Ok(values);
        }
    }
}