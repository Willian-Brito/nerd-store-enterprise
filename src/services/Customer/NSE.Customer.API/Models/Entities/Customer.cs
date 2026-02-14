using NSE.Core.DomainObjects;
using NSE.Core.Data;

namespace NSE.Customer.API.Models.Entities;

public class Customer : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string SocialNumber { get; private set; }
    public bool Deleted { get; private set; }
    public Address Address { get; private set; }
    
    // EF Constructor
    protected Customer() { }
    
    public Customer(Guid id, string name, string email, string ssn)
    {
        Id = id;
        Name = name;
        Email = new Email(email);
        SocialNumber = ssn;
        Deleted = false;
    }
    
    public void ChangeEmail(string email)
    {
        Email = new Email(email);
    }

    public void SetAddress(Address address)
    {
        Address = address;
    }
}