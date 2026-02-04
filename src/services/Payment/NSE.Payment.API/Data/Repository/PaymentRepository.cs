using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using Models = NSE.Payment.API.Models;

namespace NSE.Payment.API.Data.Repository;

public class PaymentRepository : Models.IPaymentRepository
{
    private readonly PaymentContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public PaymentRepository(PaymentContext context)
    {
        _context = context;
    }

    public void AddPayment(Models.Payment payment)
    {
        _context.Payments.Add(payment);
    }

    public void AddTransaction(Models.Transaction transaction)
    {
        _context.Transactions.Add(transaction);
    }

    public async Task<Models.Payment> GetPaymentByOrderId(Guid orderId)
    {
        return await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<IEnumerable<Models.Transaction>> GetTransactionsByOrderId(Guid orderId)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.Payment.OrderId == orderId)
            .ToListAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}