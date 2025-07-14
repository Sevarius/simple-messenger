using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Queries;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Chats.QueryHandlers;

internal sealed class ListChatsHandler : IRequestHandler<ListChats, IReadOnlyList<Chat>>
{
    public ListChatsHandler(IChatsReadOnlyRepository chatsReadOnlyRepository)
    {
        EnsureArg.IsNotNull(chatsReadOnlyRepository, nameof(chatsReadOnlyRepository));

        this.chatsReadOnlyRepository = chatsReadOnlyRepository;
    }

    private readonly IChatsReadOnlyRepository chatsReadOnlyRepository;

    public async Task<IReadOnlyList<Chat>> Handle(ListChats query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        return await this.chatsReadOnlyRepository.ListAsync(query.UserId, cancellationToken);
    }
}
