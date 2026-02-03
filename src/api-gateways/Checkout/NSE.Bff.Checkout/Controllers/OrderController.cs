using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Bff.Checkout.DTOs;
using NSE.Bff.Checkout.Services.Catalog;
using NSE.Bff.Checkout.Services.Customer;
using NSE.Bff.Checkout.Services.Order;
using NSE.Bff.Checkout.Services.ShoppingCart;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Bff.Checkout.Controllers;

[Authorize]
[Route("bff/orders")]
public class OrderController : MainController
{
    private readonly ICatalogService _catalogService;
    private readonly ICustomerService _customerService;
    private readonly IOrderService _orderService;
    private readonly IShoppingCartService _shoppingCartService;

    public OrderController(
        ICatalogService catalogService,
        IShoppingCartService shoppingCartService,
        IOrderService orderService,
        ICustomerService customerService)
    {
        _catalogService = catalogService;
        _shoppingCartService = shoppingCartService;
        _orderService = orderService;
        _customerService = customerService;
    }
    
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> AddOrder(OrderDto order)
    {
        var cart = await _shoppingCartService.GetShoppingCart();
        var products = await _catalogService.GetItems(cart.Items.Select(p => p.ProductId));
        var address = await _customerService.GetAddress();

        if (!await CheckShoppingCartProducts(cart, products)) return CustomResponse();

        PopulateOrderData(cart, address, order);

        return CustomResponse(await _orderService.FinishOrder(order));
    }

    [HttpGet("last")]
    public async Task<IActionResult> LastOrder()
    {
        var order = await _orderService.GetLastOrder();
        if (order is null)
        {
            AddErrorToStack("Order not found!");
            return CustomResponse();
        }

        return CustomResponse(order);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> CustomerList()
    {
        var orders = await _orderService.GetCustomers();

        return orders == null ? NotFound() : CustomResponse(orders);
    }

    private async Task<bool> CheckShoppingCartProducts(ShoppingCartDto shoppingCart, IEnumerable<ProductDto> products)
    {
        if (shoppingCart.Items.Count != products.Count())
        {
            var unavailableItems =
                shoppingCart.Items.Select(c => c.ProductId).Except(products.Select(p => p.Id)).ToList();

            foreach (var itemId in unavailableItems)
            {
                var cartItem = shoppingCart.Items.First(c => c.ProductId == itemId);
                AddErrorToStack(
                    $"The item {cartItem.Name} is not available at our catalog. Remove it from shoppingCart to continue shopping.");
            }

            return false;
        }

        foreach (var cartItem in shoppingCart.Items)
        {
            var catalogProduct = products.FirstOrDefault(p => p.Id == cartItem.ProductId);

            if (catalogProduct.Price != cartItem.Price)
            {
                var msgErro = $"The price of product {cartItem.Name} has changed (from: " +
                              $"{string.Format("{0:C}", cartItem.Price)} to: " +
                              $"{string.Format("{0:C}", catalogProduct.Price)}) since it has added to shoppingCart.";

                AddErrorToStack(msgErro);

                var responseRemove = await _shoppingCartService.RemoveItem(cartItem.ProductId);
                if (ResponseHasErrors(responseRemove))
                {
                    AddErrorToStack(
                        $"It was not possible to auto remove the product {cartItem.Name} from your shopping cart, _" +
                        "remove and add it again.");
                    return false;
                }

                cartItem.Price = catalogProduct.Price;
                var responseAdd = await _shoppingCartService.AddItem(cartItem);

                if (ResponseHasErrors(responseAdd))
                {
                    AddErrorToStack(
                        $"It was not possible to auto update you product {cartItem.Name} from your shopping cart, _" +
                        "add it again.");
                    return false;
                }

                CleanErrors();
                AddErrorToStack(msgErro + " We've updated your shopping cart. Check it again.");

                return false;
            }
        }

        return true;
    }

    private void PopulateOrderData(ShoppingCartDto shoppingCart, AddressDto address, OrderDto order)
    {
        order.Voucher = shoppingCart.Voucher?.Code;
        order.HasVoucher = shoppingCart.HasVoucher;
        order.Amount = shoppingCart.Total;
        order.Discount = shoppingCart.Discount;
        order.OrderItems = shoppingCart.Items;

        order.Address = address;
    }
}