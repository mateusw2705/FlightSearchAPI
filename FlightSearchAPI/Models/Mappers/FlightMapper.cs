namespace FlightSearchAPI.Models.Mappers;

public static class FlightMapper
{
    public static Flight MapToFlight(FlightDto flightDto)
    {
        return new Flight
        {
            FlightNumber = flightDto.FlightNumber,
            Airline = flightDto.Carrier,
            Origin = flightDto.OriginAirport,
            Destination = flightDto.DestinationAirport,
            DepartureTime = DateTime.Parse(flightDto.DepartureDate),
            ArrivalTime = DateTime.Parse(flightDto.ArrivalDate),
            Fare = flightDto.FarePrice
        };
    }
}
