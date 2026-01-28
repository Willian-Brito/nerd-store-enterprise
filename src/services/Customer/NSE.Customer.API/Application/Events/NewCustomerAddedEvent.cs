using NSE.Core.Messages.Base;

namespace NSE.Customer.API.Application.Events;

public class NewCustomerAddedEvent : Event
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string SocialNumber { get; private set; }
    
    public NewCustomerAddedEvent(Guid id, string name, string email, string socialNumber)
    {
        AggregateId = id;
        Id = id;
        Name = name;
        Email = email;
        SocialNumber = socialNumber;
    }
}