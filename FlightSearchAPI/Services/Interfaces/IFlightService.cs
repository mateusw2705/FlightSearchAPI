using FlightSearchAPI.Models;

namespace FlightSearchAPI.Services.Interfaces;

public interface IFlightService
{
    Task<IEnumerable<Flight>> GetFlightsAsync(string origin, string destination, DateTime date);
}
