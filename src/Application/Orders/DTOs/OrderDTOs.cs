namespace Application.Orders.DTOs;

public record CreateOrderRequest(string Type, List<OrderItemRequest> Items);

public record OrderItemRequest(Guid ProductId, int Quantity);

public record UpdateOrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderResponse(Guid Id);

public record OrderResponse(
    Guid Id,
    string Type,
    decimal Subtotal,
    decimal DiscountOrSurcharge,
    decimal Total,
    List<OrderItemResponse> Items);

public record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);
