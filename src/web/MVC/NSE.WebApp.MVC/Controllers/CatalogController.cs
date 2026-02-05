using Microsoft.AspNetCore.Mvc;
using NSE.WebApp.MVC.Controllers;
using NSE.WebApp.MVC.Services.Catalog;

namespace nse.WebApp.MVC.Controllers;

public class CatalogController : MainController
{
    private readonly ICatalogService _catalogService;

    public CatalogController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    [Route("")]
    [Route("showcase")]
    public async Task<IActionResult> Index(
        [FromQuery] int ps = 8, 
        [FromQuery] int page = 1,
        [FromQuery] string q = null
    )
    {
        var products = await _catalogService.GetAll(ps, page, q);
        ViewBag.Search = q;
        products.ReferenceAction = "Index";

        return View(products);
    }

    [HttpGet]
    [Route("products/{id}")]
    public async Task<IActionResult> ProductDetails(Guid id)
    {
        var product = await _catalogService.GetById(id);

        return View(product);
    }
}