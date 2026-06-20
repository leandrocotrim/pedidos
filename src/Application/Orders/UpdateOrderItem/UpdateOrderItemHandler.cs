using Application.Interfaces;
using Application.Orders.DTOs;

namespace Application.Orders.UpdateOrderItem;

public class UpdateOrderItemHandler(IOrderRepository orderRepository, IProductRepository productRepository)
{
    public async Task<OrderItemResponse> HandleAsync(
        Guid orderId, Guid itemId, UpdateOrderItemRequest request, CancellationToken ct = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, ct)
            ?? throw new KeyNotFoundException($"Pedido {orderId} não encontrado.");

        var product = await productRepository.GetByIdAsync(request.ProductId, ct)
            ?? throw new KeyNotFoundException($"Produto {request.ProductId} não encontrado.");

        var item = order.UpdateItem(itemId, product.Id, product.Name, request.Quantity, product.UnitPrice);
        await orderRepository.UpdateAsync(order, ct);

        return new OrderItemResponse(item.Id, item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, item.Subtotal);
    }
}
