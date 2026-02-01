using NSE.Core.Data;
using NSE.Order.Domain.Entities.Vouchers;

namespace NSE.Order.Domain.Interfaces;

public interface IVoucherRepository : IRepository<Voucher>
{
    Task<Voucher> GetVoucherByCode(string code);
    void Update(Voucher voucher);
}