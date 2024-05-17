using System.Net.Http;
using System.Text.Json;
using FlightSearchAPI.Models;
using FlightSearchAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlightSearchAPI.Services
{
    public class GolFlightService : IFlightService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GolFlightService> _logger;
        private readonly FlightMappingService _flightMappingService;

        public GolFlightService(HttpClient httpClient, ILogger<GolFlightService> logger, FlightMappingService flightMappingService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _flightMappingService = flightMappingService;
        }

        public async Task<IEnumerable<Flight>> GetFlightsAsync(string origin, string destination, DateTime date)
        {
            var response = await GetApiResponseAsync(origin, destination, date);
            var golFlights = DeserializeResponse(response);
            return _flightMappingService.MapToFlights(golFlights);
        }

        private async Task<string> GetApiResponseAsync(string origin, string destination, DateTime date)
        {
            var url = $"https://dev.reserve.com.br/airapi/gol/getavailability?origin={origin}&destination={destination}&date={date:yyyy-MM-dd}";
            return await _httpClient.GetStringAsync(url);
        }

        private List<FlightDto> DeserializeResponse(string response)
        {
            _logger.LogInformation("GOL API response: {Response}", response);
            try
            {
                return JsonSerializer.Deserialize<List<FlightDto>>(response) ?? new List<FlightDto>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing GOL API response.");
                return new List<FlightDto>();
            }
        }
    }
}
