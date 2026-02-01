using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Order.Domain.Entities.Orders;
using NSE.Order.Domain.Interfaces;
using NSE.Order.Infra.Context;
using Entities = NSE.Order.Domain.Entities.Orders;

namespace NSE.Order.Infra.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersContext _context;

    public OrderRepository(OrdersContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public DbConnection GetConnection()
    {
        return _context.Database.GetDbConnection();
    }

    public async Task<Entities.Order> GetById(Guid id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task<IEnumerable<Entities.Order>> GetCustomersById(Guid customerId)
    {
        return await _context.Orders
            .Include(p => p.OrderItems)
            .AsNoTracking()
            .Where(p => p.CustomerId == customerId)
            .ToListAsync();
    }

    public void Add(Entities.Order order)
    {
        _context.Orders.Add(order);
    }

    public void Update(Entities.Order order)
    {
        _context.Orders.Update(order);
    }


    public async Task<OrderItem> GetItemById(Guid id)
    {
        return await _context.OrderItems.FindAsync(id);
    }

    public async Task<OrderItem> GetItemByOrder(Guid orderId, Guid productId)
    {
        return await _context.OrderItems
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.OrderId == orderId);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<Entities.Order> GetLastOrder(Guid customerId)
    {
        var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

        return await _context.Orders
            .Include(i => i.OrderItems)
            .Where(o => o.CustomerId == customerId && o.DateAdded > fiveMinutesAgo && o.DateAdded <= DateTime.Now)
            .OrderByDescending(o => o.DateAdded)
            .FirstOrDefaultAsync();
    }

    public async Task<Entities.Order> GetLastAuthorizedOrder()
    {
        return await _context.Orders
            .Include(i => i.OrderItems)
            .Where(o => o.OrderStatus == OrderStatus.Authorized)
            .OrderBy(o => o.DateAdded)
            .FirstOrDefaultAsync();
    }
}