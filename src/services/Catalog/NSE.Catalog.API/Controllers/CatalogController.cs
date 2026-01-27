using Microsoft.AspNetCore.Mvc;
using NSE.Catalog.API.Data.Repository.Products;
using NSE.Catalog.API.Models;
using NSE.WebAPI.Core.Controllers;
using NSE.WebAPI.Core.Structures;

namespace DevStore.Catalog.API.Controllers;

[Route("catalog")]
public class CatalogController : MainController
{
    private readonly IProductRepository _productRepository;

    public CatalogController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    [HttpGet("products")]
    public async Task<PagedList<Product>> Index(
        [FromQuery] int ps = 8, 
        [FromQuery] int page = 1,
        [FromQuery] string q = null
    )
    {
        return await _productRepository.GetAll(ps, page, q);
    }

    [HttpGet("products/{id}")]
    public async Task<Product> Details(Guid id)
    {
        return await _productRepository.GetById(id);
    }

    [HttpGet("products/list/{ids}")]
    public async Task<IEnumerable<Product>> GetManyById(string ids)
    {
        return await _productRepository.GetProductsById(ids);
    }
}