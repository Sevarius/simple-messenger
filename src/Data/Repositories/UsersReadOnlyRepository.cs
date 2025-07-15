using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Mappings;
using Application.Repositories;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data.Repositories;

public sealed class UsersReadOnlyRepository : IUsersReadOnlyRepository
{
    public UsersReadOnlyRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public Task<UserModel[]> ListAsync(CancellationToken cancellationToken)
    {
        return this.dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .Select(user => user.ToModel())
            .ToArrayAsync(cancellationToken);
    }

    public async Task<UserModel> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        return await this.dbContext.Users
            .AsNoTracking()
            .Select(user => user.ToModel())
            .SingleOrDefaultAsync(user => user.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException($"User with ID {userId} not found.");
    }
}
