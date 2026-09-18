namespace ProductManagement.Application.Contracts;

public sealed record ProductDto(Guid Id, string Name, decimal Price, int Quantity);
public sealed record CreateProductRequest(string Name, decimal Price, int Quantity);
public sealed record UpdateProductRequest(string Name, decimal Price, int Quantity);
