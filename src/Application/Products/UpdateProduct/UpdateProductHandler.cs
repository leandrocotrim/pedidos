using Application.Interfaces;
using Application.Products.DTOs;

namespace Application.Products.UpdateProduct;

public class UpdateProductHandler(IProductRepository repository)
{
    public async Task<ProductResponse> HandleAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var product = await repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Produto {id} não encontrado.");

        product.Update(request.Name, request.UnitPrice);
        await repository.UpdateAsync(product, ct);

        return new ProductResponse(product.Id, product.Name, product.UnitPrice, product.CreatedAt);
    }
}
