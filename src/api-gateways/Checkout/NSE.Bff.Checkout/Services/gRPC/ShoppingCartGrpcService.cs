using NSE.Bff.Checkout.DTOs;
using NSE.ShoppingCart.API.Services.gRPC;

namespace NSE.Bff.Checkout.Services.gRPC;

public class ShoppingCartGrpcService : IShoppingCartGrpcService
{
    private readonly ShoppingCartOrders.ShoppingCartOrdersClient _shoppingCartClient;
    
    public ShoppingCartGrpcService(ShoppingCartOrders.ShoppingCartOrdersClient shoppingCartClient)
    {
        _shoppingCartClient = shoppingCartClient;
    }
    
    public async Task<ShoppingCartDto> GetShoppingCart()
    {
        var response = await _shoppingCartClient.GetShoppingCartAsync(new GetShoppingCartRequest());
        return MapShoppingCartProtoResponseDto(response);
    }

    private static ShoppingCartDto MapShoppingCartProtoResponseDto(
        CustomerShoppingCartClientResponse shoppingCartResponse
    )
    {
        var cartDto = new ShoppingCartDto
        {
            Total = (decimal)shoppingCartResponse.Total,
            Discount = (decimal)shoppingCartResponse.Discount,
            HasVoucher = shoppingCartResponse.Hasvoucher
        };

        if (shoppingCartResponse.Voucher != null)
        {
            cartDto.Voucher = new VoucherDTO
            {
                Code = shoppingCartResponse.Voucher.Code,
                Percentage = (decimal?)shoppingCartResponse.Voucher.Percentage,
                Discount = (decimal?)shoppingCartResponse.Voucher.Discount,
                DiscountType = shoppingCartResponse.Voucher.Discounttype
            };
        }
        
        shoppingCartResponse.Items.ToList().ForEach(item =>
        {
            cartDto.Items.Add(new ShoppingCartItemDto
            {
                Name = item.Name,
                Image = item.Image,
                ProductId = Guid.Parse(item.Productid),
                Quantity = item.Quantity,
                Price = (decimal)item.Price
            });
        });

        return cartDto;
    }
}