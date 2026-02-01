using NSE.Bff.Checkout.DTOs;

namespace NSE.Bff.Checkout.Services.Catalog;

public interface ICatalogService
{
    Task<ProductDto> GetById(Guid id);
    Task<IEnumerable<ProductDto>> GetItems(IEnumerable<Guid> ids);
}