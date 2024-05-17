using FlightSearchAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    public async Task<IActionResult> Get(
        [FromQuery] string origem,
        [FromQuery] string destino,
        [FromQuery] DateTime data)
    {
        var flights = await _flightAggregatorService.GetFlightsAsync(origem, destino, data);
        return Ok(flights);
    }
}
