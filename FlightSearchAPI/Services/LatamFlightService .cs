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

        List<LatamFlight> latamFlights;
        try
        {
            latamFlights = JsonSerializer.Deserialize<List<LatamFlight>>(response);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing LATAM API response.");
            return new List<Flight>();
        }

        if (latamFlights == null)
        {
            _logger.LogError("Deserialized LATAM flights list is null.");
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
        if (string.IsNullOrEmpty(latamFlight.DepartureDate) || string.IsNullOrEmpty(latamFlight.ArrivalDate))
        {
            throw new ArgumentException("Invalid flight data.");
        }

        return new Flight
        {
            FlightNumber = latamFlight.FlightNumber,
            Airline = latamFlight.Carrier,
            Origin = latamFlight.OriginAirport,
            Destination = latamFlight.DestinationAirport,
            DepartureTime = DateTime.Parse(latamFlight.DepartureDate),
            ArrivalTime = DateTime.Parse(latamFlight.ArrivalDate),
            Fare = latamFlight.FarePrice
        };
    }
}

public class LatamFlight
{
    public string FlightNumber { get; set; }
    public string Carrier { get; set; }
    public string DepartureDate { get; set; }
    public string ArrivalDate { get; set; }
    public string OriginAirport { get; set; }
    public string DestinationAirport { get; set; }
    public decimal FarePrice { get; set; }
}
