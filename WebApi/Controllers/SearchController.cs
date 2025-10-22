using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("available-buses")]
        public async Task<IActionResult> GetAvailableBuses([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime journeyDate)
        {
            var result = await _searchService.SearchAvailableBusesAsync(from, to, journeyDate);
            return Ok(result);
        }
    }
}
