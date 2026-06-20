namespace Application.Products.DTOs;

public record CreateProductRequest(string Name, decimal UnitPrice);

public record UpdateProductRequest(string Name, decimal UnitPrice);

public record ProductResponse(Guid Id, string Name, decimal UnitPrice, DateTime CreatedAt);
