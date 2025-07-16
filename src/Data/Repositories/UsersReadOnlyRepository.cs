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

    public async Task<UserModel[]> ListAsync(CancellationToken cancellationToken)
    {
        var result = await this.dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .Select(user => user.ToModel())
            .ToListAsync(cancellationToken);

        return result.ToArray();
    }

    public async Task<UserModel> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        return await this.dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.ToModel())
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"User with ID {userId} not found.");
    }

    public Task<UserModel[]> ListRelatedUsersAsync(Guid actorId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));

        return this.dbContext.Chats
            .AsNoTracking()
            .Where(chat => chat.Users.Any(user => user.Id == actorId))
            .SelectMany(chat => chat.Users)
            .Distinct()
            .Where(user => user.Id != actorId)
            .Select(user => user.ToModel())
            .ToArrayAsync(cancellationToken);
    }
}
