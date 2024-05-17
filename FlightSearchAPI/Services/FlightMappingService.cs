using FlightSearchAPI.Models;
using FlightSearchAPI.Models.Mappers;
using FlightSearchAPI.Models.Validators;

namespace FlightSearchAPI.Services
{
    public class FlightMappingService
    {
        public IEnumerable<Flight> MapToFlights(IEnumerable<FlightDto> flightDtos)
        {
            return flightDtos
                .Where(FlightDtoValidator.IsValid)
                .Select(FlightMapper.MapToFlight);
        }
    }
}
