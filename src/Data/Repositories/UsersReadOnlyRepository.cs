using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public sealed class UsersReadOnlyRepository : IUsersReadOnlyRepository
{
    public UsersReadOnlyRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken)
        => await this.dbContext.Users
        .AsNoTracking()
        .Where(user => !user.IsDeleted)
        .OrderBy(user => user.UserName)
        .ToListAsync(cancellationToken);
}
