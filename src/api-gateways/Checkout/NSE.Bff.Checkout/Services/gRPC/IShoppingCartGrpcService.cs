using NSE.Bff.Checkout.DTOs;

namespace NSE.Bff.Checkout.Services.gRPC;

public interface IShoppingCartGrpcService
{
    Task<ShoppingCartDto> GetShoppingCart();
}