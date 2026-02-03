using FluentValidation.Results;
using MediatR;
using NSE.Core.Messages.Base;
using NSE.Customer.API.Application.Events;
using NSE.Customer.API.Data.Models.Interfaces;
using Entities = NSE.Customer.API.Models.Entities;

namespace NSE.Customer.API.Application.Commands;

public class CustomerCommandHandler : CommandHandler,
    IRequestHandler<NewCustomerCommand, ValidationResult>,
    IRequestHandler<AddAddressCommand, ValidationResult>
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ValidationResult> Handle(AddAddressCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var address = new Entities.Address(
            message.StreetAddress, 
            message.BuildingNumber, 
            message.SecondaryAddress,
            message.Neighborhood, 
            message.ZipCode, 
            message.City, 
            message.State, 
            message.CustomerId.Value
        );
        
        _customerRepository.AddAddress(address);

        return await PersistData(_customerRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(NewCustomerCommand message, CancellationToken cancellationToken)
    {
        if (!message.IsValid()) return message.ValidationResult;

        var customer = new Entities.Customer(message.Id, message.Name, message.Email, message.SocialNumber);
        var customerExist = await _customerRepository.GetBySocialNumber(customer.SocialNumber);

        if (customerExist != null)
        {
            AddError("Already has this social number.");
            return ValidationResult;
        }

        _customerRepository.Add(customer);

        var newCustomerAddedEvent = new NewCustomerAddedEvent(
            message.Id, 
            message.Name, 
            message.Email, 
            message.SocialNumber
        );
        customer.AddEvent(newCustomerAddedEvent);

        return await PersistData(_customerRepository.UnitOfWork);
    }
}