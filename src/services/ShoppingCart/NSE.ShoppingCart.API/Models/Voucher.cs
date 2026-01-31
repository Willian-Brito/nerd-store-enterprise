namespace NSE.ShoppingCart.API.Models;

public class Voucher
{
    public decimal? Percentage { get; set; }
    public decimal? Discount { get; set; }
    public string Code { get; set; }
    public DiscountType DiscountType { get; set; }
}