using System.Net;
using Microsoft.Extensions.Options;
using NSE.Bff.Checkout.DTOs;
using NSE.Bff.Checkout.Extensions;
using NSE.Core.Communication;

namespace NSE.Bff.Checkout.Services.Order;

public class OrderService : Service, IOrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(settings.Value.OrderUrl);
    }

    public async Task<ResponseResult> FinishOrder(OrderDto order)
    {
        var orderContent = GetContent(order);
        var response = await _httpClient.PostAsync("/api/orders", orderContent);

        if (!ManageHttpResponse(response)) return await DeserializeResponse<ResponseResult>(response);

        return Ok();
    }

    public async Task<OrderDto> GetLastOrder()
    {
        var response = await _httpClient.GetAsync("/api/orders/last");

        if (response.StatusCode == HttpStatusCode.NoContent) return null;

        ManageHttpResponse(response);

        return await DeserializeResponse<OrderDto>(response);
    }

    public async Task<IEnumerable<OrderDto>> GetCustomers()
    {
        var response = await _httpClient.GetAsync("/api/orders/customers");

        if (response.StatusCode == HttpStatusCode.NoContent) return null;

        ManageHttpResponse(response);

        return await DeserializeResponse<IEnumerable<OrderDto>>(response);
    }

    public async Task<VoucherDTO> GetVoucherByCode(string code)
    {
        var response = await _httpClient.GetAsync($"/api/voucher/{code}");

        if (response.StatusCode == HttpStatusCode.NoContent) return null;

        ManageHttpResponse(response);

        return await DeserializeResponse<VoucherDTO>(response);
    }
}