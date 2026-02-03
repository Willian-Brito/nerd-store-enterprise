namespace NSE.WebApp.MVC.ViewModel;

public class OrderViewModel
{
    #region Order
    public int Code { get; set; }

    // Authorized = 1,
    // Paid = 2,
    // Refused = 3,
    // Chargeback = 4,
    // Canceled = 5
    public int Status { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }

    public decimal Discount { get; set; }
    public bool HasVoucher { get; set; }
    #endregion

    #region Items
    public List<OrderItemViewModel> OrderItems { get; set; } = new();
    #endregion

    #region Address
    public AddressViewModel Address { get; set; }
    #endregion
}