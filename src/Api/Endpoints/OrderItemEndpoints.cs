using Api.Filters;
using Application.Orders.DeleteOrderItem;
using Application.Orders.DTOs;
using Application.Orders.UpdateOrderItem;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class OrderItemEndpoints
{
    public static void MapOrderItemEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/orders/{orderId:guid}/items/{itemId:guid}", async (
            Guid orderId,
            Guid itemId,
            [FromBody] UpdateOrderItemRequest request,
            UpdateOrderItemHandler handler,
            CancellationToken ct) =>
        {
            var response = await handler.HandleAsync(orderId, itemId, request, ct);
            return Results.Ok(response);
        })
        .WithTags("Itens")
        .WithName("AtualizarItem")
        .WithSummary("Atualiza um item do pedido")
        .WithDescription("Atualiza produto e quantidade. Novo snapshot de preço é capturado e totais são recalculados.")
        .Produces<OrderItemResponse>()
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .AddEndpointFilter<ValidationFilter<UpdateOrderItemRequest>>();

        app.MapDelete("/orders/{orderId:guid}/items/{itemId:guid}", async (
            Guid orderId,
            Guid itemId,
            DeleteOrderItemHandler handler,
            CancellationToken ct) =>
        {
            await handler.HandleAsync(orderId, itemId, ct);
            return Results.NoContent();
        })
        .WithTags("Itens")
        .WithName("RemoverItem")
        .WithSummary("Remove um item do pedido")
        .WithDescription("Remove o item e recalcula os totais do pedido.")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
