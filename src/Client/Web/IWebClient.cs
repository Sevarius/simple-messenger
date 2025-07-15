using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Web.Models;

namespace Client.Web;

public interface IWebClient : IAsyncDisposable
{
    Task<EntityCreatedResponse> CreateUserAsync(string userName, CancellationToken cancellationToken);
    Task<UserResponse[]> ListUsersAsync(CancellationToken cancellationToken);
    Task<UserResponse> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<EntityCreatedResponse> CreatePrivateChatAsync(Guid actorId, CreatePrivateChatRequest request, CancellationToken cancellationToken);
    Task<ChatResponse[]> ListChatsAsync(Guid actorId, CancellationToken cancellationToken);
    Task<ChatResponse> GetChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
    Task<MessageResponse[]> ListMessagesAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken);
    Task<MessageResponse> GetMessageAsync(Guid actorId, Guid chatId, Guid messageId, CancellationToken cancellationToken);
}
