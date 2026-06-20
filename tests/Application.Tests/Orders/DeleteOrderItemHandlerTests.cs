using Application.Interfaces;
using Application.Orders.DeleteOrderItem;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;

namespace Application.Tests.Orders;

public class DeleteOrderItemHandlerTests
{
    private readonly Mock<IOrderRepository> _repo = new();
    private readonly DeleteOrderItemHandler _handler;

    public DeleteOrderItemHandlerTests() => _handler = new DeleteOrderItemHandler(_repo.Object);

    // T30
    [Fact]
    public async Task ItemNotFound_ThrowsKeyNotFoundException()
    {
        var order = new Order(OrderType.Standard, [new OrderItem(Guid.NewGuid(), "Produto", 1, 10m)]);
        _repo.Setup(r => r.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(order);

        var act = () => _handler.HandleAsync(order.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
