using Microsoft.AspNetCore.Mvc;
using NSE.Core.Bus;
using NSE.Customer.API.Application.Commands;
using NSE.Customer.API.Data.Models.Interfaces;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Customer.API.Controllers;

[Route("api/customers")]
public class CustomerController : MainController
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMessageBus _messageBus;
    private readonly IAspNetUser _user;
    
    public CustomerController(
        ICustomerRepository customerRepository, 
        IMessageBus messageBus, 
        IAspNetUser user
    )
    {
        _customerRepository = customerRepository;
        _messageBus = messageBus;
        _user = user;
    }
    
    [HttpGet("address")]
    public async Task<IActionResult> GetAddress()
    {
        var address = await _customerRepository.GetAddressById(_user.GetUserId());
        return address == null ? NotFound() : CustomResponse(address);
    }

    [HttpPost("address")]
    public async Task<IActionResult> AddAddress(AddAddressCommand address)
    {
        address.CustomerId = _user.GetUserId();
        return CustomResponse(await _messageBus.SendCommand(address));
    }
}