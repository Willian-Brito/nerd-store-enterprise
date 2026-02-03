using NSE.Core.Specification.Validation;

namespace NSE.Order.Domain.Entities.Vouchers.Specs;

public class VoucherValidation : SpecValidator<Voucher>
{
    public VoucherValidation()
    {
        var dateSpec = new VoucherDateSpecification();
        var quantitySpec = new VoucherQuantitySpecification();
        var activeSpec = new VoucherActiveSpecification();

        Add("dateSpec", new Rule<Voucher>(dateSpec, "Este voucher está expirado"));
        Add("quantitySpec", new Rule<Voucher>(quantitySpec, "Este voucher já foi utilizado"));
        Add("activeSpec", new Rule<Voucher>(activeSpec, "Este voucher não está mais ativo"));
    }
}