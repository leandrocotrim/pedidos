using Application.Interfaces;
using Application.Orders.DTOs;
using Application.Orders.UpdateOrderItem;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;

namespace Application.Tests.Orders;

public class UpdateOrderItemHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly UpdateOrderItemHandler _handler;

    public UpdateOrderItemHandlerTests() =>
        _handler = new UpdateOrderItemHandler(_orderRepo.Object, _productRepo.Object);

    // T29
    [Fact]
    public async Task ItemNotFound_ThrowsKeyNotFoundException()
    {
        var order = new Order(OrderType.Standard, [new OrderItem(Guid.NewGuid(), "Produto", 1, 10m)]);
        var product = new Product("Produto", 10m);

        _orderRepo.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(order);
        _productRepo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(product);

        var act = () => _handler.HandleAsync(order.Id, Guid.NewGuid(), new UpdateOrderItemRequest(product.Id, 1));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
