using NSE.Order.Application.DTOs;

namespace NSE.Order.Application.Queries.Order;

public interface IOrderQueries
{
    Task<OrderDto> GetLastOrder(Guid customerId);
    Task<IEnumerable<OrderDto>> GetByCustomerId(Guid customerId);
    Task<OrderDto> GetAuthorizedOrders();
}