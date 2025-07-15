using System;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Application.Repositories;

public interface IChatsReadOnlyRepository
{
    Task<ChatModel[]> ListAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
    Task<ChatModel> GetByIdAsync(Guid chatId, CancellationToken cancellationToken);
}
