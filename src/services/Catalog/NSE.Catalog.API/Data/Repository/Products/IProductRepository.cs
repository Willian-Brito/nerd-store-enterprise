using NSE.Catalog.API.Models;
using NSE.WebAPI.Core.Structures;

namespace NSE.Catalog.API.Data.Repository.Products;

public interface IProductRepository
{
    Task<PagedList<Product>> GetAll(int pageSize, int pageIndex, string query = null);
    Task<Product> GetById(Guid id);
    Task<List<Product>> GetProductsById(string ids);

    void Add(Product product);
    void Update(Product product);
}