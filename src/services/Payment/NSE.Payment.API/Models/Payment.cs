using NSE.Core.DomainObjects;

namespace NSE.Payment.API.Models;

public class Payment: Entity, IAggregateRoot
{
    public Guid OrderId { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Amount { get; set; }
    public CreditCard CreditCard { get; set; }

    // EF Relation
    public ICollection<Transaction> Transactions { get; set; }
    
    public Payment()
    {
        Transactions = new List<Transaction>();
    }
    
    public void AddTransaction(Transaction transaction)
    {
        Transactions.Add(transaction);
    }
}