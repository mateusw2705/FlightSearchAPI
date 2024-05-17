namespace FlightSearchAPI.Models;

public class FlightDto
{
    public string FlightNumber { get; set; }
    public string Carrier { get; set; }
    public string DepartureDate { get; set; }
    public string ArrivalDate { get; set; }
    public string OriginAirport { get; set; }
    public string DestinationAirport { get; set; }
    public decimal FarePrice { get; set; }
}
