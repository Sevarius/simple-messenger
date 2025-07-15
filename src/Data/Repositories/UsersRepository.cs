using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public sealed class UsersRepository : IUsersRepository
{
    public UsersRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public async Task<User> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        var user = await this.dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {userId} not found.");
        }

        return user;
    }

    public async Task<User?> FindAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        return await this.dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<User[]> ListAsync(CancellationToken cancellationToken)
    {
        var result = await this.dbContext.Users
            .Where(user => !user.IsDeleted)
            .OrderBy(user => user.UserName)
            .ToListAsync(cancellationToken);

        return result.ToArray();
    }

    public void Insert(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        this.dbContext.Users.Add(user);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await this.dbContext.SaveChangesAsync(cancellationToken);
}
