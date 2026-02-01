namespace NSE.Bff.Checkout.DTOs;

public class ShoppingCartDto
{
    public decimal Total { get; set; }
    public VoucherDTO Voucher { get; set; }
    public bool HasVoucher { get; set; }
    public decimal Discount { get; set; }
    public List<ShoppingCartItemDto> Items { get; set; } = new();
}