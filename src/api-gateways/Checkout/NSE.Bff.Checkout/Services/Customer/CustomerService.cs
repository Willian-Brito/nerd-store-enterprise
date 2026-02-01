using System.Net;
using Microsoft.Extensions.Options;
using NSE.Bff.Checkout.DTOs;
using NSE.Bff.Checkout.Extensions;

namespace NSE.Bff.Checkout.Services.Customer;

public class CustomerService : Service, ICustomerService
{
    private readonly HttpClient _httpClient;

    public CustomerService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.CustomerUrl);
    }

    public async Task<AddressDto> GetAddress()
    {
        var response = await _httpClient.GetAsync("/api/customers/address");

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        ManageHttpResponse(response);

        return await DeserializeResponse<AddressDto>(response);
    }
}