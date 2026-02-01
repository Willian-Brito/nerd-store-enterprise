using Microsoft.Extensions.Options;
using NSE.Bff.Checkout.DTOs;
using NSE.Bff.Checkout.Extensions;

namespace NSE.Bff.Checkout.Services.Catalog;

public class CatalogService : Service, ICatalogService
{
    private readonly HttpClient _httpClient;

    public CatalogService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.CatalogUrl);
    }

    public async Task<ProductDto> GetById(Guid id)
    {
        var response = await _httpClient.GetAsync($"/api/catalog/products/{id}");
        ManageHttpResponse(response);

        return await DeserializeResponse<ProductDto>(response);
    }

    public async Task<IEnumerable<ProductDto>> GetItems(IEnumerable<Guid> ids)
    {
        var idsRequest = string.Join(",", ids);
        var response = await _httpClient.GetAsync($"/api/catalog/products/list/{idsRequest}/");
        ManageHttpResponse(response);

        return await DeserializeResponse<IEnumerable<ProductDto>>(response);
    }
}