using Entities = NSE.Order.Domain.Entities.Orders;

namespace NSE.Order.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public int Code { get; set; }

    public Guid CustomerId { get; set; }
    public int Status { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }

    public decimal Discount { get; set; }
    public string Voucher { get; set; }
    public bool HasVoucher { get; set; }

    public List<OrderItemDto> OrderItems { get; set; }
    public AddressDto Address { get; set; }

    public static OrderDto ToOrderDTO(Entities.Order order)
    {
        var orderDTO = new OrderDto
        {
            Id = order.Id,
            Code = order.Code,
            Status = (int)order.OrderStatus,
            Date = order.CreatedAt,
            Amount = order.Amount,
            Discount = order.Discount,
            HasVoucher = order.HasVoucher,
            OrderItems = new List<OrderItemDto>(),
            Address = new AddressDto()
        };

        order.OrderItems.ToList().ForEach(item =>
        {
            orderDTO.OrderItems.Add(new OrderItemDto
            {
                Name = item.ProductName,
                Image = item.ProductImage,
                Quantity = item.Quantity,
                ProductId = item.ProductId,
                Price = item.Price,
                OrderId = item.OrderId
            });
        });

        orderDTO.Address = new AddressDto
        {
            StreetAddress = order.Address.StreetAddress,
            BuildingNumber = order.Address.BuildingNumber,
            SecondaryAddress = order.Address.SecondaryAddress,
            Neighborhood = order.Address.Neighborhood,
            ZipCode = order.Address.ZipCode,
            City = order.Address.City,
            State = order.Address.State
        };

        return orderDTO;
    }
}