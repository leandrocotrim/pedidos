using Application.Interfaces;
using Application.Orders.DTOs;
using Domain.Entities;
using Domain.Enums;

namespace Application.Orders.CreateOrder;

public class CreateOrderHandler(IOrderRepository orderRepository, IProductRepository productRepository)
{
    public async Task<CreateOrderResponse> HandleAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        if (!Enum.TryParse<OrderType>(request.Type, ignoreCase: true, out var orderType))
            throw new ArgumentException($"Tipo de pedido inválido: '{request.Type}'.");

        var items = new List<OrderItem>();
        foreach (var itemRequest in request.Items)
        {
            var product = await productRepository.GetByIdAsync(itemRequest.ProductId, ct)
                ?? throw new KeyNotFoundException($"Produto {itemRequest.ProductId} não encontrado.");

            items.Add(new OrderItem(product.Id, product.Name, itemRequest.Quantity, product.UnitPrice));
        }

        var order = new Order(orderType, items);
        await orderRepository.AddAsync(order, ct);

        return new CreateOrderResponse(order.Id);
    }
}
