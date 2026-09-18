using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain;
using ProductManagement.Infrastructure;

namespace ProductManagement.Infrastructure.Tests;

public class PersistenceTests
{
    [Fact]
    public async Task Product_repository_reads_products_from_database()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new AppDbContext(options);
        db.Products.Add(new Product("Mouse", 19.99m, 8));
        await db.SaveChangesAsync();

        var repository = new ProductRepository(db);

        var products = await repository.GetAllAsync(CancellationToken.None);

        Assert.Single(products);
        Assert.Equal("Mouse", products[0].Name);
    }
}