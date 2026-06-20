using Application.Interfaces;
using Application.Products.DTOs;

namespace Application.Products.GetProduct;

public class GetProductHandler(IProductRepository repository)
{
    public async Task<ProductResponse> HandleAsync(Guid id, CancellationToken ct = default)
    {
        var product = await repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Produto {id} não encontrado.");

        return new ProductResponse(product.Id, product.Name, product.UnitPrice, product.CreatedAt);
    }
}
