using System.Net.Http;
using System.Text.Json;
using FlightSearchAPI.Models;
using FlightSearchAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlightSearchAPI.Services
{
    public class LatamFlightService : IFlightService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LatamFlightService> _logger;
        private readonly FlightMappingService _flightMappingService;

        public LatamFlightService(HttpClient httpClient, ILogger<LatamFlightService> logger, FlightMappingService flightMappingService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _flightMappingService = flightMappingService;
        }

        public async Task<IEnumerable<Flight>> GetFlightsAsync(string origin, string destination, DateTime date)
        {
            var response = await GetApiResponseAsync(origin, destination, date);
            var latamFlights = DeserializeResponse(response);
            return _flightMappingService.MapToFlights(latamFlights);
        }

        private async Task<string> GetApiResponseAsync(string origin, string destination, DateTime date)
        {
            var url = $"https://dev.reserve.com.br/airapi/latam/flights?departureCity={origin}&arrivalCity={destination}&departureDate={date:yyyy-MM-dd}";
            return await _httpClient.GetStringAsync(url);
        }

        private List<FlightDto> DeserializeResponse(string response)
        {
            _logger.LogInformation("LATAM API response: {Response}", response);
            try
            {
                return JsonSerializer.Deserialize<List<FlightDto>>(response) ?? new List<FlightDto>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing LATAM API response.");
                return new List<FlightDto>();
            }
        }
    }
}
