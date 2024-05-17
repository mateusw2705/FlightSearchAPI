using FlightSearchAPI.Models;
using FlightSearchAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FlightAggregatorService : IFlightService
{
    private readonly IEnumerable<IFlightService> _flightServices;

    public FlightAggregatorService(IEnumerable<IFlightService> flightServices)
    {
        _flightServices = flightServices;
    }

    public async Task<IEnumerable<Flight>> GetFlightsAsync(string origin, string destination, DateTime date)
    {
        var tasks = _flightServices.Select(service => service.GetFlightsAsync(origin, destination, date));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(f => f)
                      .OrderBy(f => f.Fare)
                      .ThenBy(f => f.DepartureTime);
    }
}
