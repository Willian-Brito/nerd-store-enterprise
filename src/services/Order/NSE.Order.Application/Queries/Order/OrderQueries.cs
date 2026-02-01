using NSE.Order.Application.DTOs;
using NSE.Order.Domain.Interfaces;
using Entities = NSE.Order.Domain.Entities.Orders;

namespace NSE.Order.Application.Queries.Order;

public class OrderQueries: IOrderQueries
{
    private readonly IOrderRepository _orderRepository;

    public OrderQueries(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<OrderDto> GetLastOrder(Guid customerId)
    {
        var order = await _orderRepository.GetLastOrder(customerId);

        if (order is null)
            return null;

        return MapOrder(order);
    }

    public async Task<IEnumerable<OrderDto>> GetByCustomerId(Guid customerId)
    {
        var orders = await _orderRepository.GetCustomersById(customerId);
        return orders.Select(OrderDto.ToOrderDTO);
    }

    public async Task<OrderDto> GetAuthorizedOrders()
    {
        var orders = await _orderRepository.GetLastAuthorizedOrder();
        return MapOrder(orders);
    }
    
    private OrderDto MapOrder(Entities.Order order)
    {
        if (order is null)
            return null;

        var orderDto = new OrderDto
        {
            Id = order.Id,
            Code = order.Code,
            CustomerId = order.CustomerId,
            Status = (int)order.OrderStatus,
            Date = order.DateAdded,
            Amount = order.Amount,
            Discount = order.Discount,
            HasVoucher = order.HasVoucher,
            Address = new AddressDto
            {
                StreetAddress = order.Address.StreetAddress,
                Neighborhood = order.Address.Neighborhood,
                ZipCode = order.Address.ZipCode,
                City = order.Address.City,
                SecondaryAddress = order.Address.SecondaryAddress,
                State = order.Address.State,
                BuildingNumber = order.Address.BuildingNumber
            },
            OrderItems = order.OrderItems.Select(item => new OrderItemDto
            {
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                Name = item.ProductName,
                Price = item.Price,
                Image = item.ProductImage,
                Quantity = item.Quantity
            }).ToList()
        };

        return orderDto;
    }
}