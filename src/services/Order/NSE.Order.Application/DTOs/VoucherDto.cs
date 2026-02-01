namespace NSE.Order.Application.DTOs;

public class VoucherDto
{
    public decimal? Percentage { get; set; }
    public decimal? Discount { get; set; }
    public string Code { get; set; }
    public int DiscountType { get; set; }
}