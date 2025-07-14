using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Web.Models;

namespace Client.Web;

public interface IWebClient
{
    Task<EntityCreatedResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<UserResponse[]> ListUsersAsync(CancellationToken cancellationToken);
    Task<EntityCreatedResponse> CreateChatAsync(CreatePrivateChatRequest request, CancellationToken cancellationToken);
    Task<ChatResponse[]> ListChatsAsync(Guid userId, CancellationToken cancellationToken);
}
