using NSE.ShoppingCart.API.Models;

namespace NSE.ShoppingCart.UnitTests.Factories;

public static class CartItemFactory
{
    public static CartItem Create(
        Guid productId,
        string productName,
        int quantity,
        decimal price
    )
    {
        return new CartItem
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            Name = productName,
            Quantity = quantity,
            Price = price,
            Image = "product.jpg"
        };
    }
    
    public static CartItem Create()
    {
        return new CartItem
        {
            ProductId = Guid.NewGuid(),
            Name = "Mouse Gamer",
            Quantity = 1,
            Price = 100,
            Image = "mouse.jpg"
        };
    }
}