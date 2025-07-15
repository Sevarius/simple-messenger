using System;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Application.Repositories;

public interface IMessagesReadOnlyRepository
{
    Task<MessageModel[]> ListAsync(Guid chatId, CancellationToken cancellationToken);
    Task<MessageModel> GetAsync(Guid chatId, Guid messageId, CancellationToken cancellationToken);
}
