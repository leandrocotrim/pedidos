namespace Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product() { }

    public Product(string name, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        Name = name;
        UnitPrice = unitPrice;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, decimal unitPrice)
    {
        Name = name;
        UnitPrice = unitPrice;
    }
}
