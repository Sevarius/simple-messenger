using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public sealed class ChatsReadOnlyRepository : IChatsReadOnlyRepository
{
    public ChatsReadOnlyRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public async Task<IReadOnlyList<Chat>> ListAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        return await this.dbContext.Chats
            .AsNoTracking()
            .Where(chat => chat.Users.Any(user => user.Id == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return await this.dbContext.Chats
            .AsNoTracking()
            .AnyAsync(chat => chat.Id == chatId && chat.Users.Any(user => user.Id == userId), cancellationToken);
    }
}
