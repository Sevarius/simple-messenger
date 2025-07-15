using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Queries;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Chats.QueryHandlers;

internal sealed class GetChatHandler : IRequestHandler<GetChat, Chat>
{
    public GetChatHandler(IChatsReadOnlyRepository chatsReadOnlyRepository)
    {
        EnsureArg.IsNotNull(chatsReadOnlyRepository, nameof(chatsReadOnlyRepository));

        this.chatsReadOnlyRepository = chatsReadOnlyRepository;
    }

    private readonly IChatsReadOnlyRepository chatsReadOnlyRepository;

    public async Task<Chat> Handle(GetChat query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var hasAccess = await this.chatsReadOnlyRepository.IsUserInChatAsync(query.ActorId, query.ChatId, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException($"User {query.ActorId} does not have access to chat {query.ChatId}");
        }

        return await this.chatsReadOnlyRepository.GetByIdAsync(query.ChatId, cancellationToken);
    }
} 