using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages.Queries;
using Application.Repositories;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

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

    private static readonly ILogger Logger = Log.ForContext<ListMessagesHandler>();
    private readonly IMessagesReadOnlyRepository messagesReadOnlyRepository;
    private readonly IChatsReadOnlyRepository chatsReadOnlyRepository;

    public async Task<MessageModel[]> Handle(ListMessages query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        Logger.Information("Listing messages for chat {ChatId} by user {ActorId}", query.ChatId, query.ActorId);

        var hasAccess = await this.chatsReadOnlyRepository.IsUserInChatAsync(query.ActorId, query.ChatId, cancellationToken);

        if (!hasAccess)
        {
            Logger.Warning("User {ActorId} attempted to access messages in chat {ChatId} without permission", query.ActorId, query.ChatId);
            throw new UnauthorizedAccessException($"User {query.ActorId} does not have access to chat {query.ChatId}");
        }

        var messages = await this.messagesReadOnlyRepository.ListAsync(query.ChatId, cancellationToken);

        Logger.Information("Successfully retrieved {MessageCount} messages for chat {ChatId}", messages.Length, query.ChatId);

        return messages;
    }
}
