using NSE.Core.Communication;
using NSE.WebApp.MVC.ViewModel;

namespace NSE.WebApp.MVC.Services.CheckoutBff;

public interface ICheckoutBffService
{
    // Shopping cart
    Task<ShoppingCartViewModel> GetShoppingCart();
    Task<int> GetShoppingCartItemsQuantity();
    Task<ResponseResult> AddShoppingCartItem(ShoppingCartItemViewModel shoppingCart);
    Task<ResponseResult> UpdateShoppingCartItem(Guid productId, ShoppingCartItemViewModel shoppingCart);
    Task<ResponseResult> RemoveItemFromShoppingCart(Guid productId);
    Task<ResponseResult> ApplyVoucher(string voucher);

    // Order
    Task<ResponseResult> FinishOrder(TransactionViewModel transaction);
    Task<OrderViewModel> GetLastOrder();
    Task<IEnumerable<OrderViewModel>> GetCustomersById();
    TransactionViewModel MapToOrder(ShoppingCartViewModel shoppingCart, AddressViewModel address);
}