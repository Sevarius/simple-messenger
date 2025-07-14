using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories;

public interface IMessagesRepository
{
    Task<Message> GetAsync(Guid messageId, CancellationToken cancellationToken);

    Task<Message?> FindAsync(Guid messageId, CancellationToken cancellationToken);

    Task<Message[]> ListAsync(Guid chatId, CancellationToken cancellationToken);

    void Insert(Message message);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
