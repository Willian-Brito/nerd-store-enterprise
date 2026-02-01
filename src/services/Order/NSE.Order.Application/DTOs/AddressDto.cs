namespace NSE.Order.Application.DTOs;

public class AddressDto
{
    public string StreetAddress { get; set; }
    public string BuildingNumber { get; set; }
    public string SecondaryAddress { get; set; }
    public string Neighborhood { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
    public string State { get; set; }
}