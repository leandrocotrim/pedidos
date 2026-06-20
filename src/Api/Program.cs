using Api.Endpoints;
using Application.Interfaces;
using Application.Orders.CreateOrder;
using Application.Orders.DeleteOrderItem;
using Application.Orders.GetOrder;
using Application.Orders.UpdateOrderItem;
using Application.Products.CreateProduct;
using Application.Products.DeleteProduct;
using Application.Products.GetAllProducts;
using Application.Products.GetProduct;
using Application.Products.UpdateProduct;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<GetOrderHandler>();
builder.Services.AddScoped<UpdateOrderItemHandler>();
builder.Services.AddScoped<DeleteOrderItemHandler>();

builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetProductHandler>();
builder.Services.AddScoped<GetAllProductsHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<DeleteProductHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, _, _) =>
    {
        doc.Info.Title = "Pedidos API";
        doc.Info.Version = "v1";
        doc.Info.Description = "API para gerenciamento de pedidos com suporte a tipos standard, express e subscription.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

    (int status, string message) = ex switch
    {
        ArgumentException e    => (StatusCodes.Status400BadRequest, e.Message),
        KeyNotFoundException e => (StatusCodes.Status404NotFound, e.Message),
        _                      => (StatusCodes.Status500InternalServerError, "Erro interno no servidor.")
    };

    ctx.Response.StatusCode = status;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { error = message });
}));

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Pedidos API";
    options.Theme = ScalarTheme.Purple;
    options.DefaultHttpClient = new(ScalarTarget.Shell, ScalarClient.Curl);
});

app.MapProductEndpoints();
app.MapOrderEndpoints();
app.MapOrderItemEndpoints();

app.Run();

public partial class Program { }
