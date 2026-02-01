using NSE.Core.DomainObjects;
using NSE.Order.Domain.Entities.Vouchers.Specs;

namespace NSE.Order.Domain.Entities.Vouchers;

public class Voucher : Entity, IAggregateRoot
{
    public string Code { get; private set; }
    public decimal? Percentage { get; private set; }
    public decimal? Discount { get; private set; }
    public int Quantity { get; private set; }
    public VoucherDiscountType DiscountType { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public bool Active { get; private set; }
    public bool Used { get; private set; }
    
    public Voucher(
        string code, 
        decimal? percentage, 
        decimal? discount, 
        int quantity, 
        VoucherDiscountType discountType,
        DateTime expirationDate
    )
    {
        Code = code;
        Percentage = percentage;
        Discount = discount;
        Quantity = quantity;
        DiscountType = discountType;
        ExpirationDate = expirationDate;
        
        Active = true;
        Used = false;
    }
    
    public bool CanUse()
    {
        return new VoucherActiveSpecification()
            .And(new VoucherDateSpecification())
            .And(new VoucherQuantitySpecification())
            .IsSatisfiedBy(this);
    }

    public void SetAsUsed()
    {
        Active = false;
        Used = true;
        Quantity = 0;
        UsedAt = DateTime.Now;
    }

    public void GetOne()
    {
        Quantity -= 1;
        if (Quantity >= 1) return;

        SetAsUsed();
    }
}