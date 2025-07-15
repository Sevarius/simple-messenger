using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Chats.Queries;

public sealed record GetChat : IRequest<ChatModel>
{
    public GetChat(Guid actorId, Guid chatId)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        this.ActorId = actorId;
        this.ChatId = chatId;
    }

    public Guid ActorId { get; }
    public Guid ChatId { get; }
}
