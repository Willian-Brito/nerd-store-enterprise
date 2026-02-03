using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NSE.Core.Validation;

namespace NSE.WebApp.MVC.ViewModel;

public class TransactionViewModel
{
    #region Order
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public string Voucher { get; set; }
    public bool HasVoucher { get; set; }
    #endregion

    #region Items
    public List<ShoppingCartItemViewModel> Items { get; set; } = new();
    #endregion

    #region Address
    public AddressViewModel Address { get; set; }
    #endregion
    
    #region Card

    [Required(ErrorMessage = "Card number is required")]
    [DisplayName("Card number")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Please inform card holder")]
    [DisplayName("Holder")]
    public string Holder { get; set; }

    [RegularExpression(@"(0[1-9]|1[0-2])\/[0-9]{2}", ErrorMessage = "The expiration must be in form of MM/YY")]
    [CreditCardExpired(ErrorMessage = "Expired Credit Card")]
    [Required(ErrorMessage = "Credit card expiration is required")]
    [DisplayName("Expiration MM/YY")]
    public string ExpirationDate { get; set; }

    [Required(ErrorMessage = "Security code is required")]
    [DisplayName("Security Code")]
    public string SecurityCode { get; set; }

    #endregion
}