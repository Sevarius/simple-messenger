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

public sealed class MessagesReadOnlyRepository : IMessagesReadOnlyRepository
{
    public MessagesReadOnlyRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public async Task<MessageModel> GetAsync(Guid chatId, Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        return await this.dbContext.Messages
            .AsNoTracking()
            .Where(message => message.ChatId == chatId && message.Id == messageId)
            .Select(message => message.ToModel())
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"Message with ID {messageId} not found in chat {chatId}.");
    }

    public async Task<MessageModel[]> ListAsync(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return await this.dbContext.Messages
            .AsNoTracking()
            .Where(message => message.ChatId == chatId && !message.IsDeleted)
            .OrderBy(message => message.CreatedAt)
            .Select(message => message.ToModel())
            .ToArrayAsync(cancellationToken);
    }
}
