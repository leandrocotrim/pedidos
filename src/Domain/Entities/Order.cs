using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public OrderType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal DiscountOrSurcharge { get; private set; }
    public decimal Total { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public Order(OrderType type, IEnumerable<OrderItem> items)
    {
        Id = Guid.NewGuid();
        Type = type;
        CreatedAt = DateTime.UtcNow;
        _items.AddRange(items);
        RecalculateTotals();
    }

    public void AddItem(OrderItem item)
    {
        _items.Add(item);
        RecalculateTotals();
    }

    public OrderItem UpdateItem(Guid itemId, Guid productId, string productName, int quantity, decimal unitPrice)
    {
        var item = GetItem(itemId);
        item.Update(productId, productName, quantity, unitPrice);
        RecalculateTotals();
        return item;
    }

    public void RemoveItem(Guid itemId)
    {
        _items.Remove(GetItem(itemId));
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        Subtotal = _items.Sum(i => i.Subtotal);
        Total = Type switch
        {
            OrderType.Express      => Subtotal * 1.15m,
            OrderType.Subscription => Subtotal * 0.90m,
            _                      => Subtotal
        };
        DiscountOrSurcharge = Total - Subtotal;
    }

    private OrderItem GetItem(Guid itemId) =>
        _items.FirstOrDefault(i => i.Id == itemId)
        ?? throw new KeyNotFoundException($"Item {itemId} não encontrado no pedido {Id}.");
}
