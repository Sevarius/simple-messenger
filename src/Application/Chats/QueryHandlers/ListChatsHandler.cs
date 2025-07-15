using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Queries;
using Application.Repositories;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Chats.QueryHandlers;

internal sealed class ListChatsHandler : IRequestHandler<ListChats, ChatModel[]>
{
    public ListChatsHandler(IChatsReadOnlyRepository chatsReadOnlyRepository)
    {
        EnsureArg.IsNotNull(chatsReadOnlyRepository, nameof(chatsReadOnlyRepository));

        this.chatsReadOnlyRepository = chatsReadOnlyRepository;
    }

    private readonly IChatsReadOnlyRepository chatsReadOnlyRepository;

    public async Task<ChatModel[]> Handle(ListChats query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        return await this.chatsReadOnlyRepository.ListAsync(query.UserId, cancellationToken);
    }
}
