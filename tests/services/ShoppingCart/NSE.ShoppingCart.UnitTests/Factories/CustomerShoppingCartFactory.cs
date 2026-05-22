using NSE.ShoppingCart.API.Models;

namespace NSE.ShoppingCart.UnitTests.Factories;

public class CustomerShoppingCartFactory
{
    public static CustomerShoppingCart Create()
    {
        return new CustomerShoppingCart(Guid.NewGuid());
    }
}