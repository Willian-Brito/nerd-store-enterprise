using NSE.Bff.Checkout.DTOs;
using NSE.Core.Communication;

namespace NSE.Bff.Checkout.Services.ShoppingCart;

public interface IShoppingCartService
{
    Task<ShoppingCartDto> GetShoppingCart();
    Task<ResponseResult> AddItem(ShoppingCartItemDto product);
    Task<ResponseResult> UpdateItem(Guid productId, ShoppingCartItemDto shoppingCart);
    Task<ResponseResult> RemoveItem(Guid productId);
    Task<ResponseResult> ApplyVoucher(VoucherDTO voucher);
}