using Application.Interfaces;
using Application.Products.GetAllProducts;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Products;

public class GetAllProductsHandlerTests
{
    private readonly Mock<IProductRepository> _repo = new();
    private readonly GetAllProductsHandler _handler;

    public GetAllProductsHandlerTests() => _handler = new GetAllProductsHandler(_repo.Object);

    [Fact]
    public async Task ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new("Mouse", 149.90m),
            new("Teclado", 299.90m),
        };
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(products);

        var response = await _handler.HandleAsync();

        response.Should().HaveCount(2);
        response.Select(p => p.Name).Should().BeEquivalentTo(["Mouse", "Teclado"]);
    }

    [Fact]
    public async Task NoProducts_ReturnsEmptyList()
    {
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync([]);

        var response = await _handler.HandleAsync();

        response.Should().BeEmpty();
    }
}
