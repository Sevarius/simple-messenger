using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public sealed class MessagesRepository : IMessagesRepository
{
    public MessagesRepository(MessengerDbContext dbContext)
    {
        EnsureArg.IsNotNull(dbContext, nameof(dbContext));

        this.dbContext = dbContext;
    }

    private readonly MessengerDbContext dbContext;

    public async Task<Message> GetAsync(Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        var message = await this.dbContext.Messages
            .FirstOrDefaultAsync(message => message.Id == messageId, cancellationToken);

        if (message == null)
        {
            throw new InvalidOperationException($"Message with ID {messageId} not found.");
        }

        return message;
    }

    public async Task<Message?> FindAsync(Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        return await this.dbContext.Messages
            .FirstOrDefaultAsync(message => message.Id == messageId, cancellationToken);
    }

    public async Task<Message[]> ListAsync(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        var result = await this.dbContext.Messages
            .Where(message => message.ChatId == chatId && !message.IsDeleted)
            .OrderBy(message => message.CreatedAt)
            .ToListAsync(cancellationToken);

        return result.ToArray();
    }

    public void Insert(Message message)
    {
        EnsureArg.IsNotNull(message, nameof(message));

        this.dbContext.Messages.Add(message);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await this.dbContext.SaveChangesAsync(cancellationToken);
}
