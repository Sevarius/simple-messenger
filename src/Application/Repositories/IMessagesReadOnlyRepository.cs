using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories;

public interface IMessagesReadOnlyRepository
{
    Task<Message> GetAsync(Guid chatId, Guid messageId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Message>> ListAsync(Guid chatId, CancellationToken cancellationToken);
}
