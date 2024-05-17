namespace FlightSearchAPI.Models.Validators
{
    public static class FlightDtoValidator
    {
        public static bool IsValid(FlightDto flightDto)
        {
            return !string.IsNullOrEmpty(flightDto.DepartureDate) &&
                   !string.IsNullOrEmpty(flightDto.ArrivalDate);
        }
    }
}
