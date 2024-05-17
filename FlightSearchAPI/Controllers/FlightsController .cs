using Microsoft.AspNetCore.Mvc;
using FlightSearchAPI.Services.Interfaces;

namespace FlightSearchAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightAggregatorService;

        public FlightsController(IFlightService flightAggregatorService)
        {
            _flightAggregatorService = flightAggregatorService;
        }

        [HttpGet("disponibilidade")]
        public async Task<IActionResult> Get(string origem, string destino, DateTime data)
        {
            var flights = await _flightAggregatorService.GetFlightsAsync(origem, destino, data);
            return Ok(flights);
        }
    }
}
