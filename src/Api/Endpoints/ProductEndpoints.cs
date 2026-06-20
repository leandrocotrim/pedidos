using Api.Filters;
using Application.Products.CreateProduct;
using Application.Products.DeleteProduct;
using Application.Products.DTOs;
using Application.Products.GetAllProducts;
using Application.Products.GetProduct;
using Application.Products.UpdateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (GetAllProductsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.HandleAsync(ct)))
            .WithTags("Produtos")
            .WithName("ListarProdutos")
            .WithSummary("Lista todos os produtos")
            .Produces<IReadOnlyList<ProductResponse>>();

        app.MapGet("/products/{id:guid}", async (Guid id, GetProductHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.HandleAsync(id, ct)))
            .WithTags("Produtos")
            .WithName("ConsultarProduto")
            .WithSummary("Consulta um produto por ID")
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/products", async (
            [FromBody] CreateProductRequest request,
            CreateProductHandler handler,
            CancellationToken ct) =>
        {
            var response = await handler.HandleAsync(request, ct);
            return Results.Created($"/products/{response.Id}", response);
        })
        .WithTags("Produtos")
        .WithName("CriarProduto")
        .WithSummary("Cria um novo produto")
        .Produces<ProductResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .AddEndpointFilter<ValidationFilter<CreateProductRequest>>();

        app.MapPut("/products/{id:guid}", async (
            Guid id,
            [FromBody] UpdateProductRequest request,
            UpdateProductHandler handler,
            CancellationToken ct) =>
            Results.Ok(await handler.HandleAsync(id, request, ct)))
            .WithTags("Produtos")
            .WithName("AtualizarProduto")
            .WithSummary("Atualiza um produto existente")
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<UpdateProductRequest>>();

        app.MapDelete("/products/{id:guid}", async (Guid id, DeleteProductHandler handler, CancellationToken ct) =>
        {
            await handler.HandleAsync(id, ct);
            return Results.NoContent();
        })
        .WithTags("Produtos")
        .WithName("RemoverProduto")
        .WithSummary("Remove um produto")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
