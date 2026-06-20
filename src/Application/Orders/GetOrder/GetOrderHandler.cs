using Application.Interfaces;
using Application.Orders.DTOs;

namespace Application.Orders.GetOrder;

public class GetOrderHandler(IOrderRepository repository)
{
    public async Task<OrderResponse> HandleAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await repository.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException($"Pedido {orderId} não encontrado.");

        return new OrderResponse(
            order.Id,
            order.Type.ToString(),
            order.Subtotal,
            order.DiscountOrSurcharge,
            order.Total,
            order.Items.Select(i => new OrderItemResponse(
                i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice, i.Subtotal)).ToList());
    }
}
