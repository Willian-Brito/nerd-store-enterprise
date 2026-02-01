using NSE.Bff.Checkout.DTOs;
using NSE.Core.Communication;

namespace NSE.Bff.Checkout.Services.Order;

public interface IOrderService
{
    Task<ResponseResult> FinishOrder(OrderDto order);
    Task<OrderDto> GetLastOrder();
    Task<IEnumerable<OrderDto>> GetCustomers();
    Task<VoucherDTO> GetVoucherByCode(string code);
}