using NSE.Order.Application.DTOs;
using NSE.Order.Domain.Interfaces;

namespace NSE.Order.Application.Queries.Voucher;

public class VoucherQueries : IVoucherQueries
{
    private readonly IVoucherRepository _voucherRepository;

    public VoucherQueries(IVoucherRepository voucherRepository)
    {
        _voucherRepository = voucherRepository;
    }

    public async Task<VoucherDto> GetVoucher(string voucher)
    {
        var voucherDb = await _voucherRepository.GetVoucherByCode(voucher);

        if (voucherDb == null) return null;

        if (!voucherDb.CanUse()) return null;

        return new VoucherDto
        {
            Code = voucherDb.Code,
            DiscountType = (int)voucherDb.DiscountType,
            Percentage = voucherDb.Percentage,
            Discount = voucherDb.Discount
        };
    }
}