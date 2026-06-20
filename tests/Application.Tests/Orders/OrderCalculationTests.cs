using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Application.Tests.Orders;

public class OrderCalculationTests
{
    private static OrderItem Item(decimal unitPrice, int quantity = 1) =>
        new(Guid.NewGuid(), "Produto", quantity, unitPrice);

    // T24
    [Fact]
    public void Standard_Total_EqualsSubtotal()
    {
        var order = new Order(OrderType.Standard, [Item(50m, 2)]);

        order.Subtotal.Should().Be(100m);
        order.Total.Should().Be(100m);
        order.DiscountOrSurcharge.Should().Be(0m);
    }

    // T25
    [Fact]
    public void Express_Total_IsSubtotalPlus15Percent()
    {
        var order = new Order(OrderType.Express, [Item(50m, 2)]);

        order.Subtotal.Should().Be(100m);
        order.Total.Should().Be(115m);
        order.DiscountOrSurcharge.Should().Be(15m);
    }

    // T26
    [Fact]
    public void Subscription_Total_IsSubtotalMinus10Percent()
    {
        var order = new Order(OrderType.Subscription, [Item(50m, 2)]);

        order.Subtotal.Should().Be(100m);
        order.Total.Should().Be(90m);
        order.DiscountOrSurcharge.Should().Be(-10m);
    }

    // T27
    [Fact]
    public void MultipleItems_Subtotal_IsSumOfAllItems()
    {
        var order = new Order(OrderType.Standard, [
            Item(50m, 2),
            Item(20m, 3),
        ]);

        order.Subtotal.Should().Be(160m);
        order.Total.Should().Be(160m);
    }
}
