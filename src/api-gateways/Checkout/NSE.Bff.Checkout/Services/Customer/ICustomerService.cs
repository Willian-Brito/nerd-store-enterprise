using NSE.Bff.Checkout.DTOs;

namespace NSE.Bff.Checkout.Services.Customer;

public interface ICustomerService
{
    Task<AddressDto> GetAddress();
}