using Microsoft.AspNetCore.Authorization;
using NSE.ShoppingCart.API.Models;

namespace NSE.ShoppingCart.API.Endpoints;

public static class ShoppingCartEndpoints
{
    public static void MapShoppingCartEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/api/shopping-cart");
        
        endpoints.MapGet("/", [Authorize] async (
                ShoppingCart cart) => await cart.GetShoppingCart())
            .WithName("GetShoppingCart")
            .WithTags("ShoppingCart");

        endpoints.MapPost("/", [Authorize] async (
                ShoppingCart cart,
                CartItem item) => await cart.AddItem(item))
            .ProducesValidationProblem()
            .Produces<CartItem>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("AddItem")
            .WithTags("ShoppingCart");

        endpoints.MapPut("/{productId}", [Authorize] async (
                ShoppingCart cart,
                Guid productId,
                CartItem item) => await cart.UpdateItem(productId, item))
            .ProducesValidationProblem()
            .Produces<CartItem>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateItem")
            .WithTags("ShoppingCart");

        endpoints.MapDelete("/{productId}", [Authorize] async (
                ShoppingCart cart,
                Guid productId) => await cart.RemoveItem(productId))
            .ProducesValidationProblem()
            .Produces<CartItem>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("RemoveItem")
            .WithTags("ShoppingCart");

        endpoints.MapPost("/apply-voucher", [Authorize] async (
                ShoppingCart cart,
                Voucher voucher) => await cart.ApplyVoucher(voucher))
            .ProducesValidationProblem()
            .Produces<CartItem>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("ApplyVoucher")
            .WithTags("ShoppingCart");
    }
}