using Application.Interfaces;
using Application.Products.CreateProduct;
using Application.Products.DTOs;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Products;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repo = new();
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests() => _handler = new CreateProductHandler(_repo.Object);

    [Fact]
    public async Task ValidRequest_CreatesProductAndReturnsResponse()
    {
        Product? captured = null;
        _repo.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
             .Callback<Product, CancellationToken>((p, _) => captured = p)
             .Returns(Task.CompletedTask);

        var response = await _handler.HandleAsync(new CreateProductRequest("Teclado", 299.90m));

        response.Name.Should().Be("Teclado");
        response.UnitPrice.Should().Be(299.90m);
        response.Id.Should().NotBeEmpty();
        captured.Should().NotBeNull();
        _repo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
