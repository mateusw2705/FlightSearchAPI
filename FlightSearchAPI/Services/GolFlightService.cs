using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FlightSearchAPI.Models;
using FlightSearchAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class GolFlightService : IFlightService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GolFlightService> _logger;

    public GolFlightService(HttpClient httpClient, ILogger<GolFlightService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Flight>> GetFlightsAsync(string origin, string destination, DateTime date)
    {
        var response = await _httpClient.GetStringAsync($"https://dev.reserve.com.br/airapi/gol/getavailability?origin={origin}&destination={destination}&date={date:yyyy-MM-dd}");

        _logger.LogInformation("GOL API response: {Response}", response);

        var golFlights = JsonSerializer.Deserialize<List<GolFlight>>(response);

        if (golFlights == null)
        {
            _logger.LogError("Failed to deserialize GOL flights.");
            return new List<Flight>();
        }

        var flights = new List<Flight>();
        foreach (var golFlight in golFlights)
        {
            try
            {
                flights.Add(MapToFlight(golFlight));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping flight: {Flight}", golFlight);
            }
        }

        return flights;
    }

    private Flight MapToFlight(GolFlight golFlight)
    {
        if (string.IsNullOrEmpty(golFlight.DepartureTime) || string.IsNullOrEmpty(golFlight.ArrivalTime))
        {
            throw new ArgumentException("Invalid flight data.");
        }

        return new Flight
        {
            FlightNumber = golFlight.FlightNumber,
            Airline = "GOL",
            Origin = golFlight.Origin,
            Destination = golFlight.Destination,
            DepartureTime = DateTime.Parse(golFlight.DepartureTime),
            ArrivalTime = DateTime.Parse(golFlight.ArrivalTime),
            Fare = golFlight.Fare
        };
    }
}

public class GolFlight
{
    public string FlightNumber { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public string DepartureTime { get; set; }
    public string ArrivalTime { get; set; }
    public decimal Fare { get; set; }
}
