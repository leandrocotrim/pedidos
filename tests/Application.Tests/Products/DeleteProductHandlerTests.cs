using Application.Interfaces;
using Application.Products.DeleteProduct;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Products;

public class DeleteProductHandlerTests
{
    private readonly Mock<IProductRepository> _repo = new();
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests() => _handler = new DeleteProductHandler(_repo.Object);

    [Fact]
    public async Task ExistingProduct_DeletesProduct()
    {
        var product = new Product("Headset", 199.90m);
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);

        await _handler.HandleAsync(product.Id);

        _repo.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProductNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);

        var act = () => _handler.HandleAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
