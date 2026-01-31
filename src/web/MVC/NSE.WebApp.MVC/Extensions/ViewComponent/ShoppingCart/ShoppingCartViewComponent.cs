using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Services.CheckoutBff;

namespace NSE.WebApp.MVC.Extensions.ViewComponent.ShoppingCart;

public class ShoppingCartViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
{
    // private readonly ICheckoutBffService _checkoutBffService;
    
    // public ShoppingCartViewComponent(ICheckoutBffService checkoutBffService)
    // {
    //     _checkoutBffService = checkoutBffService;
    // }
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        // return View(await _checkoutBffService.GetShoppingCartItemsQuantity());
        return View(2);
    }
}