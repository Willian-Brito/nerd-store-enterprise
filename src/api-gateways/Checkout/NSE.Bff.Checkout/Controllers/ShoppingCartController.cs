using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Bff.Checkout.DTOs;
using NSE.Bff.Checkout.Services.Catalog;
using NSE.Bff.Checkout.Services.gRPC;
using NSE.Bff.Checkout.Services.Order;
using NSE.Bff.Checkout.Services.ShoppingCart;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Bff.Checkout.Controllers;

[Authorize]
[Route("bff/orders/shopping-cart")]
public class ShoppingCartController : MainController
{
    private readonly ICatalogService _catalogService;
    private readonly IOrderService _orderService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IShoppingCartGrpcService _shoppingCartGrpcService;

    public ShoppingCartController(
        IShoppingCartService shoppingCartService,
        ICatalogService catalogService,
        IOrderService orderService,
        IShoppingCartGrpcService shoppingCartGrpcService
    )
    {
        _shoppingCartService = shoppingCartService;
        _catalogService = catalogService;
        _orderService = orderService;
        _shoppingCartGrpcService = shoppingCartGrpcService;
    }
    
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        return CustomResponse(await _shoppingCartGrpcService.GetShoppingCart());
    }

    [HttpGet]
    [Route("quantity")]
    public async Task<int> GetCartItemsQuantity()
    {
        var shoppingCart = await _shoppingCartGrpcService.GetShoppingCart();
        return shoppingCart?.Items.Sum(i => i.Quantity) ?? 0;
    }

    [HttpPost]
    [Route("items")]
    public async Task<IActionResult> AddItem(ShoppingCartItemDto shoppingCartProductItem)
    {
        var product = await _catalogService.GetById(shoppingCartProductItem.ProductId);

        await ValidateShoppingCartItem(product, shoppingCartProductItem.Quantity, true);
        if (!ValidOperation()) return CustomResponse();

        shoppingCartProductItem.Name = product.Name;
        shoppingCartProductItem.Price = product.Price;
        shoppingCartProductItem.Image = product.Image;

        var response = await _shoppingCartService.AddItem(shoppingCartProductItem);

        return CustomResponse(response);
    }

    [HttpPut]
    [Route("items/{productId}")]
    public async Task<IActionResult> UpdateCartIem(Guid productId, ShoppingCartItemDto shoppingCartProductItem)
    {
        var product = await _catalogService.GetById(productId);

        await ValidateShoppingCartItem(product, shoppingCartProductItem.Quantity);
        if (!ValidOperation()) return CustomResponse();

        var response = await _shoppingCartService.UpdateItem(productId, shoppingCartProductItem);

        return CustomResponse(response);
    }

    [HttpDelete]
    [Route("items/{productId}")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        var product = await _catalogService.GetById(productId);

        if (product == null)
        {
            AddErrorToStack("Product not found!");
            return CustomResponse();
        }

        var response = await _shoppingCartService.RemoveItem(productId);

        return CustomResponse(response);
    }

    [HttpPost]
    [Route("apply-voucher")]
    public async Task<IActionResult> ApplyVoucher([FromBody] string voucherCode)
    {
        var voucher = await _orderService.GetVoucherByCode(voucherCode);
        if (voucher is null)
        {
            AddErrorToStack("Voucher is invalid or not found!");
            return CustomResponse();
        }

        var response = await _shoppingCartService.ApplyVoucher(voucher);

        return CustomResponse(response);
    }

    private async Task ValidateShoppingCartItem(ProductDto product, int quantity, bool addProduct = false)
    {
        if (product is null) AddErrorToStack("Product not found!");
        if (quantity < 1) AddErrorToStack($"Should have at least one unit of product {product.Name}");

        var shoppingCart = await _shoppingCartService.GetShoppingCart();
        var cartItem = shoppingCart.Items.FirstOrDefault(p => p.ProductId == product.Id);

        if (cartItem != null && addProduct && cartItem.Quantity + quantity > product.Stock)
        {
            AddErrorToStack($"The product {product.Name} has {product.Stock} units at stock, you got {quantity}");
            return;
        }

        if (quantity > product.Stock)
            AddErrorToStack($"The product {product.Name} has {product.Stock} units at stock, you got {quantity}");
    }

    [HttpGet("test-auth")]
    [Authorize]
    public ActionResult TestAuth()
    {
        return Ok("pong");
    }
}