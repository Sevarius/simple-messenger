using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Models;
using Client.Web.Models;

namespace Client.Web;

public interface IWebClient : IAsyncDisposable
{
    Task<UserModel> CreateUserAsync(string userName, CancellationToken cancellationToken);
    Task<UserModel[]> ListUsersAsync(CancellationToken cancellationToken);
    Task<UserModel> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<ChatModel> CreatePrivateChatAsync(Guid actorId, CreatePrivateChatRequest request, CancellationToken cancellationToken);
    Task<ChatModel[]> ListChatsAsync(Guid actorId, CancellationToken cancellationToken);
    Task<ChatModel> GetChatAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken);
    Task<MessageModel[]> ListMessagesAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken);
    Task<MessageModel> GetMessageAsync(Guid actorId, Guid chatId, Guid messageId, CancellationToken cancellationToken);
}
