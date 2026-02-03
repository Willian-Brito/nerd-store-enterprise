using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Order.Domain.Entities.Vouchers;
using NSE.Order.Domain.Interfaces;
using NSE.Order.Infra.Context;

namespace NSE.Order.Infra.Repository;

public class VoucherRepository : IVoucherRepository
{
    private readonly OrdersContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public VoucherRepository(OrdersContext context)
    {
        _context = context;
    }

    public async Task<Voucher> GetVoucherByCode(string code)
    {
        return await _context.Vouchers.FirstOrDefaultAsync(p => p.Code == code);
    }

    public void Update(Voucher voucher)
    {
        _context.Vouchers.Update(voucher);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}