using Application.Interfaces;
using Application.Products.DTOs;
using Domain.Entities;

namespace Application.Products.CreateProduct;

public class CreateProductHandler(IProductRepository repository)
{
    public async Task<ProductResponse> HandleAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var product = new Product(request.Name, request.UnitPrice);
        await repository.AddAsync(product, ct);
        return new ProductResponse(product.Id, product.Name, product.UnitPrice, product.CreatedAt);
    }
}
