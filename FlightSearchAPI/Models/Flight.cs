namespace FlightSearchAPI.Models;

public class Flight
{
    public string FlightNumber { get; set; }
    public string Airline { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Fare { get; set; }
}
