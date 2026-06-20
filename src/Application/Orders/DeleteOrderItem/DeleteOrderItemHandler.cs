using Application.Interfaces;

namespace Application.Orders.DeleteOrderItem;

public class DeleteOrderItemHandler(IOrderRepository repository)
{
    public async Task HandleAsync(Guid orderId, Guid itemId, CancellationToken ct = default)
    {
        var order = await repository.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException($"Pedido {orderId} não encontrado.");

        order.RemoveItem(itemId);
        await repository.UpdateAsync(order, ct);
    }
}
