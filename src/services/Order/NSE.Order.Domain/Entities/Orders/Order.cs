using NSE.Core.DomainObjects;
using NSE.Core.Data;
using NSE.Order.Domain.Entities.Vouchers;

namespace NSE.Order.Domain.Entities.Orders;

public class Order : AuditableEntity, IAggregateRoot
{
    public int Code { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? VoucherId { get; private set; }
    public bool HasVoucher { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Amount { get; private set; }    
    public OrderStatus OrderStatus { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public Address Address { get; private set; }

    // EF Rel.
    public Voucher Voucher { get; private set; }
    
    private readonly List<OrderItem> _orderItems;
    
    // EF Constructor
    protected Order() { }

    public Order(
        Guid customerId, 
        decimal amount, 
        List<OrderItem> orderItems,
        bool hasVoucher = false, 
        decimal discount = 0, 
        Guid? voucherId = null
    )
    {
        CustomerId = customerId;
        Amount = amount;
        _orderItems = orderItems;

        Discount = discount;
        HasVoucher = hasVoucher;
        VoucherId = voucherId;
    }
    
    public void Authorize()
    {
        OrderStatus = OrderStatus.Authorized;
    }

    public void Cancel()
    {
        OrderStatus = OrderStatus.Canceled;
    }

    public void Finish()
    {
        OrderStatus = OrderStatus.Paid;
    }

    public void AssociateVoucher(Voucher voucher)
    {
        HasVoucher = true;
        VoucherId = voucher.Id;
        Voucher = voucher;
    }

    public void SetAddress(Address address)
    {
        Address = address;
    }

    public void CalculateOrderAmount()
    {
        Amount = OrderItems.Sum(p => p.CalculateAmount());
        CalculateAmount();
    }

    public void CalculateAmount()
    {
        if (!HasVoucher) return;

        decimal discount = 0;
        var amount = Amount;

        if (Voucher.DiscountType == VoucherDiscountType.Percentage)
        {
            if (Voucher.Percentage.HasValue)
            {
                discount = amount * Voucher.Percentage.Value / 100;
                amount -= discount;
            }
        }
        else
        {
            if (Voucher.Discount.HasValue)
            {
                discount = Voucher.Discount.Value;
                amount -= discount;
            }
        }

        Amount = amount < 0 ? 0 : amount;
        Discount = discount;
    }
}