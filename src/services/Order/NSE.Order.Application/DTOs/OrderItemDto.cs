using NSE.Order.Domain.Entities.Orders;

namespace NSE.Order.Application.DTOs;

public class OrderItemDto
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Image { get; set; }
    public int Quantity { get; set; }

    public static OrderItem ToOrderItem(OrderItemDto orderItemDto)
    {
        var entity = new OrderItem(
            orderItemDto.ProductId, 
            orderItemDto.Name, 
            orderItemDto.Quantity,
            orderItemDto.Price, 
            orderItemDto.Image
        );
        return entity;
    }
}