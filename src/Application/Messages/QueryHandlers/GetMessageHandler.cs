using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages.Queries;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Messages.QueryHandlers;

internal sealed class GetMessageHandler : IRequestHandler<GetMessage, Message>
{
    public GetMessageHandler(
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

    public async Task<Message> Handle(GetMessage query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var hasAccess = await this.chatsReadOnlyRepository.IsUserInChatAsync(query.ActorId, query.ChatId, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException($"User {query.ActorId} does not have access to chat {query.ChatId}");
        }

        return await this.messagesReadOnlyRepository.GetAsync(query.ChatId, query.MessageId, cancellationToken);
    }
}
