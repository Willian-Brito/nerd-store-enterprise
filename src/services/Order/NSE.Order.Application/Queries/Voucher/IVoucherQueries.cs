using NSE.Order.Application.DTOs;

namespace NSE.Order.Application.Queries.Voucher;

public interface IVoucherQueries
{
    Task<VoucherDto> GetVoucher(string voucher);
}