using Api.Filters;
using Application.Orders.CreateOrder;
using Application.Orders.DTOs;
using Application.Orders.GetOrder;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (
            [FromBody] CreateOrderRequest request,
            CreateOrderHandler handler,
            CancellationToken ct) =>
        {
            var response = await handler.HandleAsync(request, ct);
            return Results.Created($"/orders/{response.Id}", response);
        })
        .WithTags("Pedidos")
        .WithName("CriarPedido")
        .WithSummary("Cria um novo pedido")
        .WithDescription("Recebe tipo e lista de itens (productId + quantity). O snapshot do produto é capturado no momento da criação.")
        .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .AddEndpointFilter<ValidationFilter<CreateOrderRequest>>();

        app.MapGet("/orders/{orderId:guid}", async (
            Guid orderId,
            GetOrderHandler handler,
            CancellationToken ct) =>
        {
            var response = await handler.HandleAsync(orderId, ct);
            return Results.Ok(response);
        })
        .WithTags("Pedidos")
        .WithName("ConsultarPedido")
        .WithSummary("Consulta o resumo de um pedido")
        .WithDescription("Retorna totais armazenados no momento da criação: subtotal, desconto/acréscimo e total.")
        .Produces<OrderResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
