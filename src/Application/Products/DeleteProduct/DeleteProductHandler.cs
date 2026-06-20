using Application.Interfaces;

namespace Application.Products.DeleteProduct;

public class DeleteProductHandler(IProductRepository repository)
{
    public async Task HandleAsync(Guid id, CancellationToken ct = default)
    {
        var product = await repository.GetByIdAsync(id, ct)
            ?? throw new KeyNotFoundException($"Produto {id} não encontrado.");

        await repository.DeleteAsync(product, ct);
    }
}
