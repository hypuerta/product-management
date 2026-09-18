using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain;

namespace ProductManagement.Infrastructure.Tests;

public class UserRepositoryTests
{
    [Fact]
    public async Task GetByEmailAsync_returns_user_when_it_exists()
    {
        await using var context = CreateContext();
        var user = new User("test@example.com", "hash-123");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetByEmailAsync("test@example.com", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("hash-123", result.PasswordHash);
    }

    [Fact]
    public async Task GetByEmailAsync_returns_null_when_it_does_not_exist()
    {
        await using var context = CreateContext();
        var repository = new UserRepository(context);

        var result = await repository.GetByEmailAsync("missing@example.com", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_marks_user_for_insertion()
    {
        await using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = new User("new@example.com", "hash-abc");

        await repository.AddAsync(user, CancellationToken.None);

        var entry = context.Entry(user);
        Assert.Equal(EntityState.Added, entry.State);
    }

    [Fact]
    public async Task AddAsync_and_SaveChangesAsync_persist_user()
    {
        var databaseName = Guid.NewGuid().ToString();
        var user = new User("persist@example.com", "hash-persist");

        await using (var writeContext = CreateContext(databaseName))
        {
            var repository = new UserRepository(writeContext);

            await repository.AddAsync(user, CancellationToken.None);
            await repository.SaveChangesAsync(CancellationToken.None);
        }

        await using (var readContext = CreateContext(databaseName))
        {
            var saved = await readContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            Assert.NotNull(saved);
            Assert.Equal("persist@example.com", saved!.Email);
            Assert.Equal("hash-persist", saved.PasswordHash);
        }
    }

    [Fact]
    public async Task SaveChangesAsync_persists_pending_context_changes()
    {
        var databaseName = Guid.NewGuid().ToString();
        var user = new User("contextsave@example.com", "hash-context");

        await using (var writeContext = CreateContext(databaseName))
        {
            writeContext.Users.Add(user);
            var repository = new UserRepository(writeContext);

            await repository.SaveChangesAsync(CancellationToken.None);
        }

        await using (var readContext = CreateContext(databaseName))
        {
            var saved = await readContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
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