using Application.Interfaces;
using Application.Products.DTOs;
using Application.Products.UpdateProduct;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Products;

public class UpdateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repo = new();
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests() => _handler = new UpdateProductHandler(_repo.Object);

    [Fact]
    public async Task ExistingProduct_UpdatesAndReturnsResponse()
    {
        var product = new Product("Mouse", 149.90m);
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);

        var response = await _handler.HandleAsync(product.Id, new UpdateProductRequest("Mouse Gamer", 299.90m));

        response.Name.Should().Be("Mouse Gamer");
        response.UnitPrice.Should().Be(299.90m);
        _repo.Verify(r => r.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProductNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);

        var act = () => _handler.HandleAsync(Guid.NewGuid(), new UpdateProductRequest("X", 10m));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
