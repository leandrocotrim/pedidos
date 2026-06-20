using Application.Interfaces;
using Application.Orders.GetOrder;
using FluentAssertions;
using Moq;

namespace Application.Tests.Orders;

public class GetOrderHandlerTests
{
    private readonly Mock<IOrderRepository> _repo = new();
    private readonly GetOrderHandler _handler;

    public GetOrderHandlerTests() => _handler = new GetOrderHandler(_repo.Object);

    // T28
    [Fact]
    public async Task OrderNotFound_ThrowsKeyNotFoundException()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Domain.Entities.Order?)null);

        var act = () => _handler.HandleAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
