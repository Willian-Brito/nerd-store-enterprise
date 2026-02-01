using Microsoft.Extensions.Options;
using NSE.Bff.Checkout.DTOs;
using NSE.Bff.Checkout.Extensions;
using NSE.Core.Communication;

namespace NSE.Bff.Checkout.Services.ShoppingCart;

public class ShoppingCartService : Service, IShoppingCartService
{
    private readonly HttpClient _httpClient;

    public ShoppingCartService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.ShoppingCartUrl);
    }

    public async Task<ShoppingCartDto> GetShoppingCart()
    {
        var response = await _httpClient.GetAsync("/api/shopping-cart");
        ManageHttpResponse(response);

        return await DeserializeResponse<ShoppingCartDto>(response);
    }

    public async Task<ResponseResult> AddItem(ShoppingCartItemDto product)
    {
        var itemContent = GetContent(product);
        var response = await _httpClient.PostAsync("/api/shopping-cart", itemContent);

        if (!ManageHttpResponse(response)) return await DeserializeResponse<ResponseResult>(response);

        return Ok();
    }

    public async Task<ResponseResult> UpdateItem(Guid productId, ShoppingCartItemDto shoppingCart)
    {
        var itemContent = GetContent(shoppingCart);
        var response = await _httpClient.PutAsync($"/api/shopping-cart/{shoppingCart.ProductId}", itemContent);

        if (!ManageHttpResponse(response)) return await DeserializeResponse<ResponseResult>(response);

        return Ok();
    }

    public async Task<ResponseResult> RemoveItem(Guid productId)
    {
        var response = await _httpClient.DeleteAsync($"/api/shopping-cart/{productId}");

        if (!ManageHttpResponse(response)) return await DeserializeResponse<ResponseResult>(response);

        return Ok();
    }

    public async Task<ResponseResult> ApplyVoucher(VoucherDTO voucher)
    {
        var itemContent = GetContent(voucher);
        var response = await _httpClient.PostAsync("/api/shopping-cart/apply-voucher/", itemContent);

        if (!ManageHttpResponse(response)) return await DeserializeResponse<ResponseResult>(response);

        return Ok();
    }
}