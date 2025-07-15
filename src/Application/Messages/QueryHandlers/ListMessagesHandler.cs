using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages.Queries;
using Application.Repositories;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Messages.QueryHandlers;

internal sealed class ListMessagesHandler : IRequestHandler<ListMessages, MessageModel[]>
{
    public ListMessagesHandler(
        IMessagesReadOnlyRepository messagesReadOnlyRepository,
        IChatsReadOnlyRepository chatsReadOnlyRepository)
    {
        EnsureArg.IsNotNull(messagesReadOnlyRepository, nameof(messagesReadOnlyRepository));
        EnsureArg.IsNotNull(chatsReadOnlyRepository, nameof(chatsReadOnlyRepository));

        this.messagesReadOnlyRepository = messagesReadOnlyRepository;
        this.chatsReadOnlyRepository = chatsReadOnlyRepository;
    }

    private readonly IMessagesReadOnlyRepository messagesReadOnlyRepository;
    private readonly IChatsReadOnlyRepository chatsReadOnlyRepository;

    public async Task<MessageModel[]> Handle(ListMessages query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var hasAccess = await this.chatsReadOnlyRepository.IsUserInChatAsync(query.ActorId, query.ChatId, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException($"User {query.ActorId} does not have access to chat {query.ChatId}");
        }

        return await this.messagesReadOnlyRepository.ListAsync(query.ChatId, cancellationToken);
    }
}
