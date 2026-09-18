using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain;

namespace ProductManagement.Infrastructure.Tests;

public class ProductRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_returns_products_ordered_by_name()
    {
        await using var context = CreateContext();
        context.Products.AddRange(
            new Product("Mouse", 20m, 5),
            new Product("Keyboard", 50m, 2),
            new Product("Adapter", 10m, 10));
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        var result = await repository.GetAllAsync(CancellationToken.None);

        Assert.Equal(3, result.Count);
        Assert.Collection(
            result,
            p => Assert.Equal("Adapter", p.Name),
            p => Assert.Equal("Keyboard", p.Name),
            p => Assert.Equal("Mouse", p.Name));
    }

    [Fact]
    public async Task GetByIdAsync_returns_product_when_it_exists()
    {
        await using var context = CreateContext();
        var product = new Product("Monitor", 199.99m, 3);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        var result = await repository.GetByIdAsync(product.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result!.Id);
        Assert.Equal("Monitor", result.Name);
        Assert.Equal(199.99m, result.Price);
        Assert.Equal(3, result.Quantity);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_it_does_not_exist()
    {
        await using var context = CreateContext();
        var repository = new ProductRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_and_SaveChangesAsync_persist_product()
    {
        var product = new Product("Headset", 79.99m, 6);
        var databaseName = Guid.NewGuid().ToString();

        await using (var writeContext = CreateContext(databaseName))
        {
            var repository = new ProductRepository(writeContext);

            await repository.AddAsync(product, CancellationToken.None);
            await repository.SaveChangesAsync(CancellationToken.None);
        }

        await using (var readContext = CreateContext(databaseName))
        {
            var saved = await readContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

            Assert.NotNull(saved);
            Assert.Equal("Headset", saved!.Name);
            Assert.Equal(79.99m, saved.Price);
            Assert.Equal(6, saved.Quantity);
        }
    }

    [Fact]
    public async Task DeleteAsync_and_SaveChangesAsync_remove_product()
    {
        var product = new Product("Webcam", 59.99m, 8);
        var databaseName = Guid.NewGuid().ToString();

        await using (var seedContext = CreateContext(databaseName))
        {
            seedContext.Products.Add(product);
            await seedContext.SaveChangesAsync();
        }

        await using (var deleteContext = CreateContext(databaseName))
        {
            var repository = new ProductRepository(deleteContext);
            var existing = await deleteContext.Products.FirstAsync(p => p.Id == product.Id);

            await repository.DeleteAsync(existing, CancellationToken.None);
            await repository.SaveChangesAsync(CancellationToken.None);
        }

        await using (var verifyContext = CreateContext(databaseName))
        {
            var deleted = await verifyContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.Null(deleted);
        }
    }

    [Fact]
    public async Task SaveChangesAsync_persists_pending_context_changes()
    {
        var product = new Product("Microphone", 99.99m, 4);
        var databaseName = Guid.NewGuid().ToString();

        await using (var writeContext = CreateContext(databaseName))
        {
            writeContext.Products.Add(product);
            var repository = new ProductRepository(writeContext);

            await repository.SaveChangesAsync(CancellationToken.None);
        }

        await using (var readContext = CreateContext(databaseName))
        {
            var saved = await readContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            Assert.NotNull(saved);
        }
    }

    private static AppDbContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}