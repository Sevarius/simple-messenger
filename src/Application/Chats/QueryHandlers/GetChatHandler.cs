using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Queries;
using Application.Repositories;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

namespace Application.Chats.QueryHandlers;

internal sealed class GetChatHandler : IRequestHandler<GetChat, ChatModel>
{
    public GetChatHandler(IChatsReadOnlyRepository chatsReadOnlyRepository)
    {
        EnsureArg.IsNotNull(chatsReadOnlyRepository, nameof(chatsReadOnlyRepository));

        this.chatsReadOnlyRepository = chatsReadOnlyRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<GetChatHandler>();
    private readonly IChatsReadOnlyRepository chatsReadOnlyRepository;

    public async Task<ChatModel> Handle(GetChat query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        Logger.Information("Getting chat {ChatId} for user {ActorId}", query.ChatId, query.ActorId);

        var hasAccess = await this.chatsReadOnlyRepository.IsUserInChatAsync(query.ActorId, query.ChatId, cancellationToken);

        if (!hasAccess)
        {
            Logger.Warning("User {ActorId} attempted to access chat {ChatId} without permission", query.ActorId, query.ChatId);
            throw new UnauthorizedAccessException($"User {query.ActorId} does not have access to chat {query.ChatId}");
        }

        var result = await this.chatsReadOnlyRepository.GetAsync(query.ChatId, cancellationToken);

        Logger.Information("Successfully retrieved chat {ChatId} for user {ActorId}", query.ChatId, query.ActorId);

        return result;
    }
}
