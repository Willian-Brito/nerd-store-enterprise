using System.Net;
using Microsoft.AspNetCore.Mvc;
using NSE.Order.Application.DTOs;
using NSE.Order.Application.Queries.Voucher;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Order.API.Controllers;

[Route("api/voucher")]
public class VoucherController : MainController
{
    private readonly IVoucherQueries _voucherQueries;
    
    public VoucherController(IVoucherQueries voucherQueries)
    {
        _voucherQueries = voucherQueries;
    }
    
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(VoucherDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> GetByCode(string code)
    {
        if (string.IsNullOrEmpty(code)) return NotFound();

        var voucher = await _voucherQueries.GetVoucher(code);
        return voucher == null ? NoContent() : CustomResponse(voucher);
    }
}