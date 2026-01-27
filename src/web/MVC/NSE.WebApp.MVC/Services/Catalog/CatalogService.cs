using Microsoft.Extensions.Options;
using NSE.WebApp.MVC.Extensions;
using NSE.WebApp.MVC.Models;

namespace NSE.WebApp.MVC.Services.Catalog;

public class CatalogService : Service, ICatalogService
{
    private readonly HttpClient _httpClient;

    public CatalogService(
        HttpClient httpClient,
        IOptions<AppSettings> settings
    )
    {
        httpClient.BaseAddress = new Uri(settings.Value.CatalogUrl);
        _httpClient = httpClient;
    }

    public async Task<ProductViewModel> GetById(Guid id)
    {
        var response = await _httpClient.GetAsync($"/catalog/products/{id}");
        ManageResponseErrors(response);

        return await DeserializeResponse<ProductViewModel>(response);
    }

    public async Task<PagedViewModel<ProductViewModel>> GetAll(int pageSize, int pageIndex, string query = null)
    {
        var response = await _httpClient.GetAsync($"/catalog/products?ps={pageSize}&page={pageIndex}&q={query}");
        ManageResponseErrors(response);

        return await DeserializeResponse<PagedViewModel<ProductViewModel>>(response);
    }
}