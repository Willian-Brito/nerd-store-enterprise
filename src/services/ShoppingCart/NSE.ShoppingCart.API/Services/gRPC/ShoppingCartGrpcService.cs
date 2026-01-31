// using Grpc.Core;
// using Microsoft.EntityFrameworkCore;
// using NSE.Security.Identity.User;
// using NSE.ShoppingCart.API.Data;
// using NSE.ShoppingCart.API.Models;
//
// namespace NSE.ShoppingCart.API.Services.gRPC;
//
// public class ShoppingCartGrpcService
// {
//     private readonly ShoppingCartContext _context;
//     private readonly IAspNetUser _user;
//     
//     
//     public ShoppingCartGrpcService(IAspNetUser user, ShoppingCartContext context)
//     {
//         _user = user;
//         _context = context;
//     }
//     
//     public override async Task<CustomerShoppingCartClientResponse> GetShoppingCart(
//         GetShoppingCartRequest request,
//         ServerCallContext context
//     )
//     {
//         var shoppingCart = await GetShoppingCartClient() ?? new CustomerShoppingCart();
//         return MapShoppingCartClientToProtoResponse(shoppingCart);
//     }
//     
//     private async Task<CustomerShoppingCart> GetShoppingCartClient()
//     {
//         return await _context.CustomerShoppingCart
//             .Include(c => c.Items)
//             .FirstOrDefaultAsync(c => c.CustomerId == _user.GetUserId());
//     }
//
//     private static CustomerShoppingCartClientResponse MapShoppingCartClientToProtoResponse(
//         CustomerShoppingCart shoppingCart
//     )
//     {
//         var shoppingCartResponse = new CustomerShoppingCartClientResponse
//         {
//             Id = shoppingCart.Id.ToString(),
//             Customerid = shoppingCart.CustomerId.ToString(),
//             Total = (double)shoppingCart.Total,
//             Discount = (double)shoppingCart.Discount,
//             Hasvoucher = shoppingCart.HasVoucher
//         };
//
//         if (shoppingCart.Voucher != null)
//         {
//             shoppingCartResponse.Voucher = new VoucherResponse
//             {
//                 Code = shoppingCart.Voucher.Code,
//                 Percentage = (double?)shoppingCart.Voucher.Percentage ?? 0,
//                 Discount = (double?)shoppingCart.Voucher.Discount ?? 0,
//                 Discounttype = (int)shoppingCart.Voucher.DiscountType
//             };
//         }
//
//         shoppingCart.Items.ForEach(item =>
//         {
//             var shoppingCartItemResponse = new ShoppingCartItemResponse
//             {
//                 Id = item.Id.ToString(),
//                 Name = item.Name,
//                 Image = item.Image,
//                 Productid = item.ProductId.ToString(),
//                 Quantity = item.Quantity,
//                 Price = (double)item.Price
//             };
//             
//             shoppingCartResponse.Items.Add(shoppingCartItemResponse);
//         });
//
//         return shoppingCartResponse;
//     }
// }