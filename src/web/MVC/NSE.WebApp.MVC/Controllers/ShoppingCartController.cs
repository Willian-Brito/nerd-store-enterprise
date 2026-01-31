using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NSE.WebApp.MVC.Controllers;

[Authorize]
[Route("shopping-cart")]
public class ShoppingCartController : MainController
{
    [Route("")]
    public async Task<IActionResult> Index()
    {
        // return View(await _checkoutBffService.GetShoppingCart());
        return View("2");
    }
}