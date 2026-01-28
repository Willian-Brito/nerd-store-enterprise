using NSE.Core.DomainObjects;

namespace NSE.Customer.API.Models.Entities;

public class Address : Entity
{
    public string StreetAddress { get; private set; }
    public string BuildingNumber { get; private set; }
    public string SecondaryAddress { get; private set; }
    public string Neighborhood { get; private set; }
    public string ZipCode { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public Guid CustomerId { get; private set; }

    // EF Relation
    public Customer Customer { get; protected set; }
    
    // EF Constructor
    protected Address() { }
    
    public Address(
        string streetAddress, 
        string buildingNumber, 
        string secondaryAddress, 
        string neighborhood,
        string zipCode, 
        string city, 
        string state, 
        Guid customerId
    )
    {
        StreetAddress = streetAddress;
        BuildingNumber = buildingNumber;
        SecondaryAddress = secondaryAddress;
        Neighborhood = neighborhood;
        ZipCode = zipCode;
        City = city;
        State = state;
        CustomerId = customerId;
    }
}