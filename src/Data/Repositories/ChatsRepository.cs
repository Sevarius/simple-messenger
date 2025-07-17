using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public sealed class ChatsRepository : IChatsRepository
{
    public ChatsRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public async Task<Chat> GetAsync(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        var chat = await this.dbContext.Chats
            .Include(chat => chat.Users)
            .Include(chat => chat.UserChatReadStatuses)
            .SingleOrDefaultAsync(chat => chat.Id == chatId, cancellationToken);

        if (chat is null)
        {
            throw new InvalidOperationException($"Chat with ID {chatId} not found.");
        }

        return chat;
    }

    public async Task<TChat> GetConcreteAsync<TChat>(Guid chatId, CancellationToken cancellationToken) where TChat : Chat
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        var chat = await this.dbContext.Set<TChat>()
            .Include(chat => chat.Users)
            .Include(chat => chat.UserChatReadStatuses)
            .SingleOrDefaultAsync(chat => chat.Id == chatId, cancellationToken);

        if (chat == null)
        {
            throw new InvalidOperationException($"Chat with ID {chatId} not found.");
        }

        return chat;
    }

    public async Task<Chat?> FindAsync(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return await this.dbContext.Chats
            .Include(chat => chat.Users)
            .Include(chat => chat.UserChatReadStatuses)
            .SingleOrDefaultAsync(chat => chat.Id == chatId, cancellationToken);
    }

    public async Task<TChat?> FindConcreteAsync<TChat>(Guid chatId, CancellationToken cancellationToken) where TChat : Chat
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return await this.dbContext.Set<TChat>()
            .Include(chat => chat.Users)
            .Include(chat => chat.UserChatReadStatuses)
            .SingleOrDefaultAsync(chat => chat.Id == chatId, cancellationToken);
    }

    public async Task<Chat[]> ListAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        var result = await this.dbContext.Chats
            .Include(chat => chat.Users)
            .Include(chat => chat.UserChatReadStatuses)
            .Where(chat => chat.Users.Any(user => user.Id == userId))
            .ToListAsync(cancellationToken);

        return result.ToArray();
    }

    public void Insert(Chat chat)
    {
        EnsureArg.IsNotNull(chat, nameof(chat));

        this.dbContext.Chats.Add(chat);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await this.dbContext.SaveChangesAsync(cancellationToken);
}
