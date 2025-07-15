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

public sealed class ChatsReadOnlyRepository : IChatsReadOnlyRepository
{
    public ChatsReadOnlyRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public async Task<ChatModel[]> ListAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        return await this.dbContext.Chats
            .AsNoTracking()
            .Include(chat => chat.Users)
            .Where(chat => chat.Users.Any(user => user.Id == userId))
            .Select(chat => chat.ToModel())
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return await this.dbContext.Chats
            .AsNoTracking()
            .AnyAsync(chat => chat.Id == chatId && chat.Users.Any(user => user.Id == userId), cancellationToken);
    }

    public async Task<ChatModel> GetByIdAsync(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return await this.dbContext.Chats
            .AsNoTracking()
            .Include(chat => chat.Users)
            .Where(chat => chat.Id == chatId)
            .Select(chat => chat.ToModel())
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"Chat with ID {chatId} not found");
    }
}
