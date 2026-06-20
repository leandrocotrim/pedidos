using Application.Interfaces;
using Application.Products.GetProduct;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Products;

public class GetProductHandlerTests
{
    private readonly Mock<IProductRepository> _repo = new();
    private readonly GetProductHandler _handler;

    public GetProductHandlerTests() => _handler = new GetProductHandler(_repo.Object);

    [Fact]
    public async Task ExistingProduct_ReturnsResponse()
    {
        var product = new Product("Mouse", 149.90m);
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);

        var response = await _handler.HandleAsync(product.Id);

        response.Id.Should().Be(product.Id);
        response.Name.Should().Be("Mouse");
        response.UnitPrice.Should().Be(149.90m);
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
