using System.Net;
using Microsoft.Extensions.Options;
using NSE.Core.Communication;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.ViewModel;

namespace NSE.WebApp.MVC.Services.Customer;

public class CustomerService : Service, ICustomerService
{
    private readonly HttpClient _httpClient;

    public CustomerService(HttpClient httpClient, IOptions<AppSettings> settings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.CustomerUrl);
    }
    
    public async Task<AddressViewModel> GetAddress()
    {
        var response = await _httpClient.GetAsync("/api/customers/address");

        if (response.StatusCode == HttpStatusCode.NotFound) return null;

        ManageResponseErrors(response);

        return await DeserializeResponse<AddressViewModel>(response);
    }

    public async Task<ResponseResult> AddAddress(AddressViewModel address)
    {
        var enderecoContent = GetContent(address);

        var response = await _httpClient.PostAsync("/api/customers/address", enderecoContent);

        if (!ManageResponseErrors(response)) return await DeserializeResponse<ResponseResult>(response);

        return ReturnOk();
    }
}