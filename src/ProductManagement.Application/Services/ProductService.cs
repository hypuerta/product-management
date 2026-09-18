using ProductManagement.Application.Contracts;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain;

namespace ProductManagement.Application.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken) =>
        (await repository.GetAllAsync(cancellationToken)).Select(ToDto).ToArray();

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        (await repository.GetByIdAsync(id, cancellationToken)) is { } product ? ToDto(product) : null;

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.Price, request.Quantity);
        await repository.AddAsync(product, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return ToDto(product);
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null) return null;
        product.Update(request.Name, request.Price, request.Quantity);
        await repository.SaveChangesAsync(cancellationToken);
        return ToDto(product);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null) return false;
        await repository.DeleteAsync(product, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProductDto ToDto(Product product) => new(product.Id, product.Name, product.Price, product.Quantity);
}
