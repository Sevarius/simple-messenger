using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories;

public interface IChatsReadOnlyRepository
{
    Task<IReadOnlyList<Chat>> ListAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
}
