using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories;

public interface IChatsRepository
{
    Task<Chat> GetAsync(Guid chatId, CancellationToken cancellationToken);
    Task<TChat> GetConcreteAsync<TChat>(Guid chatId, CancellationToken cancellationToken) where TChat : Chat;

    Task<Chat?> FindAsync(Guid chatId, CancellationToken cancellationToken);
    Task<TChat?> FindConcreteAsync<TChat>(Guid chatId, CancellationToken cancellationToken) where TChat : Chat;

    Task<Chat[]> ListAsync(Guid userId, CancellationToken cancellationToken);

    void Insert(Chat chat);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
