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

internal sealed class GetMessageHandler : IRequestHandler<GetMessage, MessageModel>
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

    private static readonly ILogger Logger = Log.ForContext<GetMessageHandler>();
    private readonly IMessagesReadOnlyRepository messagesReadOnlyRepository;
    private readonly IChatsReadOnlyRepository chatsReadOnlyRepository;

    public async Task<MessageModel> Handle(GetMessage query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        Logger.Information("Getting message {MessageId} from chat {ChatId} for user {ActorId}", query.MessageId, query.ChatId, query.ActorId);

        var hasAccess = await this.chatsReadOnlyRepository.IsUserInChatAsync(query.ActorId, query.ChatId, cancellationToken);

        if (!hasAccess)
        {
            Logger.Warning("User {ActorId} attempted to access message {MessageId} in chat {ChatId} without permission", query.ActorId, query.MessageId, query.ChatId);
            throw new UnauthorizedAccessException($"User {query.ActorId} does not have access to chat {query.ChatId}");
        }

        var result = await this.messagesReadOnlyRepository.GetAsync(query.ChatId, query.MessageId, cancellationToken);

        Logger.Information("Successfully retrieved message {MessageId} from chat {ChatId} for user {ActorId}", query.MessageId, query.ChatId, query.ActorId);

        return result;
    }
}
