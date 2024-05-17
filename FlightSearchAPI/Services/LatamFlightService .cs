using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FlightSearchAPI.Models;
using FlightSearchAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class LatamFlightService : IFlightService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LatamFlightService> _logger;

    public LatamFlightService(HttpClient httpClient, ILogger<LatamFlightService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Flight>> GetFlightsAsync(string origin, string destination, DateTime date)
    {
        var response = await _httpClient.GetStringAsync($"https://dev.reserve.com.br/airapi/latam/flights?departureCity={origin}&arrivalCity={destination}&departureDate={date:yyyy-MM-dd}");

        _logger.LogInformation("LATAM API response: {Response}", response);

        var latamFlights = JsonSerializer.Deserialize<List<LatamFlight>>(response);

        if (latamFlights == null)
        {
            _logger.LogError("Failed to deserialize LATAM flights.");
            return new List<Flight>();
        }

        var flights = new List<Flight>();
        foreach (var latamFlight in latamFlights)
        {
            try
            {
                flights.Add(MapToFlight(latamFlight));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping flight: {Flight}", latamFlight);
            }
        }

        return flights;
    }

    private Flight MapToFlight(LatamFlight latamFlight)
    {
        if (string.IsNullOrEmpty(latamFlight.DepartureTime) || string.IsNullOrEmpty(latamFlight.ArrivalTime))
        {
            throw new ArgumentException("Invalid flight data.");
        }

        return new Flight
        {
            FlightNumber = latamFlight.FlightNumber,
            Airline = "LATAM",
            Origin = latamFlight.Origin,
            Destination = latamFlight.Destination,
            DepartureTime = DateTime.Parse(latamFlight.DepartureTime),
            ArrivalTime = DateTime.Parse(latamFlight.ArrivalTime),
            Fare = latamFlight.Fare
        };
    }
}

public class LatamFlight
{
    public string FlightNumber { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public string DepartureTime { get; set; }
    public string ArrivalTime { get; set; }
    public decimal Fare { get; set; }
}
