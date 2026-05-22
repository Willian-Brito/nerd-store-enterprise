using NSE.Order.Application.Commands;
using NSE.Order.Application.DTOs;

namespace NSE.Order.UnitTests.Factories;

public static class AddOrderCommandFactory
{
    public static AddOrderCommand Create()
    {
        return new AddOrderCommand
        {
            CustomerId = Guid.NewGuid(),
            Amount = 100,
            Holder = "Willian Brito",
            CardNumber = "4111111111111111",
            ExpirationDate = "12/2030",
            SecurityCode = "123",
            HasVoucher = false,
            Discount = 0,
            OrderItems = new List<OrderItemDto>
            {
                new OrderItemDto()
                {
                    OrderId = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Name = "Mouse Gamer",
                    Price = 100,
                    Image = "",
                    Quantity = 1
                }
            },
            Address = new AddressDto
            {
                StreetAddress = "Rua XPTO",
                BuildingNumber = "100",
                Neighborhood = "Centro",
                ZipCode = "00000-000",
                City = "São Paulo",
                State = "SP"
            }
        };
    }
}