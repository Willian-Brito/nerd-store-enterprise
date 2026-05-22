using FluentAssertions;
using NSE.Order.Domain.Entities.Vouchers;
using NSE.Order.Domain.Entities.Vouchers.Specs;
using NSE.Order.UnitTests.Factories;

namespace NSE.Order.UnitTests.Domain;

public class VoucherTests
{
    [Fact(DisplayName = "Deve permitir uso quando voucher estiver válido")]
    public void Should_Allow_Use_When_Voucher_Is_Valid()
    {
        // Arrange
        var voucher = VoucherFactory.Create();

        // Act
        var result = voucher.CanUse();

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Não deve permitir uso quando voucher estiver expirado")]
    public void Should_Not_Allow_Use_When_Voucher_Is_Expired()
    {
        // Arrange
        var voucher = new Voucher(
            code: "DESCONTO-10",
            percentage: 10,
            discount: null,
            quantity: 0,
            discountType: VoucherDiscountType.Percentage,
            expirationDate: DateTime.UtcNow.AddDays(-1)
        );

        // Act
        var result = voucher.CanUse();

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Não deve permitir uso quando voucher estiver inativo")]
    public void Should_Not_Allow_Use_When_Voucher_Is_Inactive()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        voucher.Disable();

        // Act
        var result = voucher.CanUse();

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Não deve permitir uso quando voucher já foi utilizado")]
    public void Should_Not_Allow_Use_When_Voucher_Has_Already_Been_Used()
    {
        // Arrange
        var voucher = VoucherFactory.Create();        
        voucher.SetAsUsed();

        // Act
        var result = voucher.CanUse();

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Não deve permitir uso quando quantidade for zero")]
    public void Should_Not_Allow_Use_When_Quantity_Is_Zero()
    {
        // Arrange
        var voucher = new Voucher(
            code: "DESCONTO-10",
            percentage: 10,
            discount: null,
            quantity: 0,
            discountType: VoucherDiscountType.Percentage,
            expirationDate: DateTime.UtcNow.AddDays(30)
        );
        
        // Act
        var result = voucher.CanUse();

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve marcar voucher como utilizado")]
    public void Should_Set_Voucher_As_Used()
    {
        // Arrange
        var voucher = VoucherFactory.Create();

        // Act
        voucher.SetAsUsed();

        // Assert
        voucher.Active.Should().BeFalse();
        voucher.Used.Should().BeTrue();
        voucher.Quantity.Should().Be(0);
        voucher.UsedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve debitar quantidade do voucher")]
    public void Should_Debit_Voucher_Quantity()
    {
        // Arrange
        var voucher = new Voucher(
            code: "DESCONTO-10",
            percentage: 10,
            discount: null,
            quantity: 5,
            discountType: VoucherDiscountType.Percentage,
            expirationDate: DateTime.UtcNow.AddDays(30)
        );

        // Act
        voucher.DebitQuantity();

        // Assert
        voucher.Quantity.Should().Be(4);
        voucher.Used.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve marcar voucher como utilizado quando quantidade zerar")]
    public void Should_Set_Voucher_As_Used_When_Quantity_Reaches_Zero()
    {
        // Arrange
        var voucher = new Voucher(
            code: "DESCONTO-10",
            percentage: 10,
            discount: null,
            quantity: 1,
            discountType: VoucherDiscountType.Percentage,
            expirationDate: DateTime.UtcNow.AddDays(30)
        );

        // Act
        voucher.DebitQuantity();

        // Assert
        voucher.Quantity.Should().Be(0);
        voucher.Active.Should().BeFalse();
        voucher.Used.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve validar especificação de data válida")]
    public void Should_Validate_Valid_Date_Specification()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        var specification = new VoucherDateSpecification();

        // Act
        var result = specification.IsSatisfiedBy(voucher);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve invalidar especificação de data expirada")]
    public void Should_Invalidate_Expired_Date_Specification()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        var newExpirationDate = DateTime.UtcNow.AddDays(-100);
        
        voucher.ChangeExpirationDate(newExpirationDate);

        var specification = new VoucherDateSpecification();

        // Act
        var result = specification.IsSatisfiedBy(voucher);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve validar especificação de quantidade")]
    public void Should_Validate_Quantity_Specification()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        var specification = new VoucherQuantitySpecification();

        // Act
        var result = specification.IsSatisfiedBy(voucher);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve invalidar especificação de quantidade")]
    public void Should_Invalidate_Quantity_Specification()
    {
        // Arrange
        var voucher = new Voucher(
            code: "DESCONTO-10",
            percentage: 10,
            discount: null,
            quantity: 0,
            discountType: VoucherDiscountType.Percentage,
            expirationDate: DateTime.UtcNow.AddDays(30)
        );

        var specification = new VoucherQuantitySpecification();

        // Act
        var result = specification.IsSatisfiedBy(voucher);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve validar especificação de voucher ativo")]
    public void Should_Validate_Active_Voucher_Specification()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        var specification = new VoucherActiveSpecification();

        // Act
        var result = specification.IsSatisfiedBy(voucher);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve invalidar especificação de voucher inativo")]
    public void Should_Invalidate_Inactive_Voucher_Specification()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        voucher.Disable();

        var specification = new VoucherActiveSpecification();

        // Act
        var result = specification.IsSatisfiedBy(voucher);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve invalidar especificação de voucher utilizado")]
    public void Should_Invalidate_Used_Voucher_Specification()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        voucher.SetAsUsed();

        var specification = new VoucherActiveSpecification();

        // Act
        var result = specification.IsSatisfiedBy(voucher);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve validar voucher através do VoucherValidation")]
    public void Should_Validate_Voucher_Using_VoucherValidation()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        var validator = new VoucherValidation();

        // Act
        var result = validator.Validate(voucher);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Deve retornar erro quando voucher estiver expirado")]
    public void Should_Return_Error_When_Voucher_Is_Expired()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        var newExpirationDate = DateTime.UtcNow.AddDays(-100);
        voucher.ChangeExpirationDate(newExpirationDate);

        var validator = new VoucherValidation();

        // Act
        var result = validator.Validate(voucher);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.ErrorMessage == "Este voucher está expirado");
    }

    [Fact(DisplayName = "Deve retornar erro quando voucher estiver sem quantidade")]
    public void Should_Return_Error_When_Voucher_Has_No_Quantity()
    {
        // Arrange
        var voucher = new Voucher(
            code: "DESCONTO-10",
            percentage: 10,
            discount: null,
            quantity: 0,
            discountType: VoucherDiscountType.Percentage,
            expirationDate: DateTime.UtcNow.AddDays(30)
        );

        var validator = new VoucherValidation();

        // Act
        var result = validator.Validate(voucher);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.ErrorMessage == "Este voucher já foi utilizado");
    }

    [Fact(DisplayName = "Deve retornar erro quando voucher estiver inativo")]
    public void Should_Return_Error_When_Voucher_Is_Inactive()
    {
        // Arrange
        var voucher = VoucherFactory.Create();
        voucher.Disable();

        var validator = new VoucherValidation();

        // Act
        var result = validator.Validate(voucher);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x =>
            x.ErrorMessage == "Este voucher não está mais ativo");
    }
}