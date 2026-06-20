using Application.Interfaces;
using Application.Products.DTOs;

namespace Application.Products.GetAllProducts;

public class GetAllProductsHandler(IProductRepository repository)
{
    public async Task<IReadOnlyList<ProductResponse>> HandleAsync(CancellationToken ct = default)
    {
        var products = await repository.GetAllAsync(ct);
        return products.Select(p => new ProductResponse(p.Id, p.Name, p.UnitPrice, p.CreatedAt)).ToList();
    }
}
