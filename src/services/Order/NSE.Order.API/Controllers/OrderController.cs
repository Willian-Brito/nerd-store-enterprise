using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Core.Bus;
using NSE.Order.Application.Commands;
using NSE.Order.Application.DTOs;
using NSE.Order.Application.Queries.Order;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Order.API.Controllers;

[Authorize]
[Route("api/orders")]
public class OrderController : MainController
{
    private readonly IMessageBus _messageBus;
    private readonly IOrderQueries _orderQueries;
    private readonly IAspNetUser _user;

    public OrderController(
        IMessageBus messageBus,
        IAspNetUser user,
        IOrderQueries orderQueries
    )
    {
        _messageBus = messageBus;
        _user = user;
        _orderQueries = orderQueries;
    }
    
    [HttpPost("")]
    public async Task<IActionResult> AddOrder(AddOrderCommand order)
    {
        order.CustomerId = _user.GetUserId();
        var result = await _messageBus.SendCommand(order);
        return CustomResponse(result);
    }

    [HttpGet("last")]
    public async Task<ActionResult<OrderDto>> LastOrder()
    {
        var customerId = _user.GetUserId();
        var order = await _orderQueries.GetLastOrder(customerId);

        return order == null ? NoContent() : CustomResponse(order);
    }

    [HttpGet("customers")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> Customers()
    {
        var customerId = _user.GetUserId();
        var orders = await _orderQueries.GetByCustomerId(customerId);

        return orders == null ? NoContent() : CustomResponse(orders);
    }
}