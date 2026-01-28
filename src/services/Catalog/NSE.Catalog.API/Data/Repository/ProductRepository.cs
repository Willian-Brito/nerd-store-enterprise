using Microsoft.EntityFrameworkCore;
using NSE.Catalog.API.Data.Models.Interfaces;
using NSE.Core.Data;
using NSE.Catalog.API.Models.Entities;
using NSE.WebAPI.Core.Extensions;
using NSE.WebAPI.Core.Structures;

namespace NSE.Catalog.API.Data.Repository;

public class ProductRepository : IProductRepository
{
    private readonly CatalogContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ProductRepository(CatalogContext context)
    {
        _context = context;
    }
    
    public async Task<PagedList<Product>> GetAll(int pageSize, int pageNumber, string query = null)
    {
        var catalogQuery = _context.Products.AsQueryable();

        var catalog = await catalogQuery.AsNoTrackingWithIdentityResolution()
            .Where(x => EF.Functions.Like(x.Name, $"%{query}%"))
            .OrderBy(x => x.Name)
            .ToListAsync();
        
        return catalog.ToPagedList(pageNumber, pageSize);
    }

    public async Task<Product> GetById(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<List<Product>> GetProductsById(string ids)
    {
        var idsGuid = ids.Split(',')
            .Select(id => (Ok: Guid.TryParse(id, out var x), Value: x));

        if (!idsGuid.All(nid => nid.Ok)) return new List<Product>();

        var idsValue = idsGuid.Select(id => id.Value);

        return await _context.Products.AsNoTracking()
            .Where(p => idsValue.Contains(p.Id) && p.Active).ToListAsync();
    }

    public void Add(Product product)
    {
        _context.Products.Add(product);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}