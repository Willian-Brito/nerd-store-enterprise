using NSE.Order.Domain.Entities.Vouchers;

namespace NSE.Order.UnitTests.Factories;

public static class VoucherFactory
{
    public static Voucher Create()
    {
        return new Voucher(
            code: "DESCONTO-10",
            percentage: 10,
            discount: null,
            quantity: 10,
            discountType: VoucherDiscountType.Percentage,
            expirationDate: DateTime.UtcNow.AddDays(30)
        );
    }
}