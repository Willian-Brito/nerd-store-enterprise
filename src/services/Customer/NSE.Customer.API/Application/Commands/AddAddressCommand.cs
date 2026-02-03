using FluentValidation;
using NSE.Core.Messages.Base;
using System.Text.Json.Serialization;

namespace NSE.Customer.API.Application.Commands;

public class AddAddressCommand : Command
{
    [JsonIgnore]
    public Guid? CustomerId { get; set; }
    public string StreetAddress { get; set; }
    public string BuildingNumber { get; set; }
    public string SecondaryAddress { get; set; }
    public string Neighborhood { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
    public string State { get; set; }

    public AddAddressCommand() { }
    
    public AddAddressCommand(
        Guid customerId, 
        string streetAddress, 
        string buildingNumber, 
        string secondaryAddress,
        string neighborhood, 
        string zipCode, 
        string city, 
        string state
    )
    {
        AggregateId = customerId;
        CustomerId = customerId;
        StreetAddress = streetAddress;
        BuildingNumber = buildingNumber;
        SecondaryAddress = secondaryAddress;
        Neighborhood = neighborhood;
        ZipCode = zipCode;
        City = city;
        State = state;
    }
    
    public override bool IsValid()
    {
        ValidationResult = new AddressValidation().Validate(this);
        return ValidationResult.IsValid;
    }
    
    public class AddressValidation : AbstractValidator<AddAddressCommand>
    {
        public AddressValidation()
        {
            RuleFor(c => c.StreetAddress)
                .NotEmpty()
                .WithMessage("Street Address must be set");

            RuleFor(c => c.BuildingNumber)
                .NotEmpty()
                .WithMessage("Building number must be set");

            RuleFor(c => c.ZipCode)
                .NotEmpty()
                .WithMessage("Zip code must be set");

            RuleFor(c => c.Neighborhood)
                .NotEmpty()
                .WithMessage("Neighborhood must be set");

            RuleFor(c => c.City)
                .NotEmpty()
                .WithMessage("City must be set");

            RuleFor(c => c.State)
                .NotEmpty()
                .WithMessage("State must be set");
        }
    }
}