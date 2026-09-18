using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain;

namespace ProductManagement.Infrastructure;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) => db.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    public Task AddAsync(User user, CancellationToken cancellationToken) { db.Users.Add(user); return Task.CompletedTask; }
    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
