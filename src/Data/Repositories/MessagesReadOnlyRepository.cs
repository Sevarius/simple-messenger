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

public sealed class MessagesReadOnlyRepository : IMessagesReadOnlyRepository
{
    public MessagesReadOnlyRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public async Task<Message> GetAsync(Guid chatId, Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        return await this.dbContext.Messages
            .AsNoTracking()
            .Where(message => message.ChatId == chatId && message.Id == messageId && !message.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"Message with ID {messageId} not found in chat {chatId}.");
    }

    public async Task<IReadOnlyList<Message>> ListAsync(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return await this.dbContext.Messages
            .AsNoTracking()
            .Where(message => message.ChatId == chatId && !message.IsDeleted)
            .OrderBy(message => message.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
