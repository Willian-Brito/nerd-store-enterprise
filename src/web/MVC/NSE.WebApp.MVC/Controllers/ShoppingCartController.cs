using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Services.CheckoutBff;
using NSE.WebApp.MVC.ViewModel;

namespace NSE.WebApp.MVC.Controllers;

[Authorize]
[Route("shopping-cart")]
public class ShoppingCartController : MainController
{
    private readonly ICheckoutBffService _checkoutBffService;
    
    public ShoppingCartController(ICheckoutBffService checkoutBffService)
    {
        _checkoutBffService = checkoutBffService;
    }
    
    [Route("")]
    public async Task<IActionResult> Index()
    {
        return View(await _checkoutBffService.GetShoppingCart());
    }
    
    [HttpPost]
    [Route("add-item")]
    public async Task<IActionResult> AddItem(ShoppingCartItemViewModel shoppingCartItem)
    {
        var response = await _checkoutBffService.AddShoppingCartItem(shoppingCartItem);

        if (ResponseHasErrors(response)) return View("Index", await _checkoutBffService.GetShoppingCart());

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Route("update-item")]
    public async Task<IActionResult> UpdateItem(Guid productId, int quantity)
    {
        var item = new ShoppingCartItemViewModel { ProductId = productId, Quantity = quantity };
        var response = await _checkoutBffService.UpdateShoppingCartItem(productId, item);

        if (ResponseHasErrors(response)) return View("Index", await _checkoutBffService.GetShoppingCart());

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Route("remove-item")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        var response = await _checkoutBffService.RemoveItemFromShoppingCart(productId);

        if (ResponseHasErrors(response)) return View("Index", await _checkoutBffService.GetShoppingCart());

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Route("apply-voucher")]
    public async Task<IActionResult> ApplyVoucher(string voucherCode)
    {
        var response = await _checkoutBffService.ApplyVoucher(voucherCode);

        if (ResponseHasErrors(response)) return View("Index", await _checkoutBffService.GetShoppingCart());

        return RedirectToAction("Index");
    }
}