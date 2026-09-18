using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain;

namespace ProductManagement.Infrastructure;

public sealed class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken) => await context.Products.AsNoTracking().OrderBy(product => product.Name).ToListAsync(cancellationToken);
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => context.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    public Task AddAsync(Product product, CancellationToken cancellationToken) { context.Products.Add(product); return Task.CompletedTask; }
    public Task DeleteAsync(Product product, CancellationToken cancellationToken) { context.Products.Remove(product); return Task.CompletedTask; }
    public Task SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
