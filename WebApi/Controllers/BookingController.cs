using Application.Services;
using Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Application.Contracts.DTOs;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("seat-plan/{busScheduleId}")]
        public async Task<IActionResult> GetSeatPlan(Guid busScheduleId)
        {
            var result = await _bookingService.GetSeatPlanAsync(busScheduleId);
            return Ok(result);
        }

        [HttpPost("book-seat")]
        public async Task<IActionResult> BookSeat([FromBody] BookSeatInputDto input)
        {
            var result = await _bookingService.BookSeatAsync(input);
            return Ok(result);
        }
    }
}
