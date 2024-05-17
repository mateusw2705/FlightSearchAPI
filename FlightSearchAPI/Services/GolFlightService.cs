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

        List<GolFlight> golFlights;
        try
        {
            golFlights = JsonSerializer.Deserialize<List<GolFlight>>(response);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing GOL API response.");
            return new List<Flight>();
        }

        if (golFlights == null)
        {
            _logger.LogError("Deserialized GOL flights list is null.");
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
        if (string.IsNullOrEmpty(golFlight.DepartureDate) || string.IsNullOrEmpty(golFlight.ArrivalDate))
        {
            throw new ArgumentException("Invalid flight data.");
        }

        return new Flight
        {
            FlightNumber = golFlight.FlightNumber,
            Airline = golFlight.Carrier,
            Origin = golFlight.OriginAirport,
            Destination = golFlight.DestinationAirport,
            DepartureTime = DateTime.Parse(golFlight.DepartureDate),
            ArrivalTime = DateTime.Parse(golFlight.ArrivalDate),
            Fare = golFlight.FarePrice
        };
    }
}

public class GolFlight
{
    public string FlightNumber { get; set; }
    public string Carrier { get; set; }
    public string DepartureDate { get; set; }
    public string ArrivalDate { get; set; }
    public string OriginAirport { get; set; }
    public string DestinationAirport { get; set; }
    public decimal FarePrice { get; set; }
}
