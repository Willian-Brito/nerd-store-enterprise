using System.Data.Common;
using NSE.Core.Data;
using NSE.Order.Domain.Entities.Orders;

namespace NSE.Order.Domain.Interfaces;

public interface IOrderRepository : IRepository<Entities.Orders.Order>
{
    Task<Entities.Orders.Order> GetById(Guid id);
    Task<IEnumerable<Entities.Orders.Order>> GetCustomersById(Guid customerId);
    void Add(Entities.Orders.Order order);
    void Update(Entities.Orders.Order order);
    DbConnection GetConnection();
    Task<Entities.Orders.Order> GetLastOrder(Guid customerId);
    Task<Entities.Orders.Order> GetLastAuthorizedOrder();

    /* Order Item */
    Task<OrderItem> GetItemById(Guid id);
    Task<OrderItem> GetItemByOrder(Guid orderId, Guid productId);
}