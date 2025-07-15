using System;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Chats.Queries;

public sealed record GetChat : IRequest<Chat>
{
    public GetChat(Guid chatId, Guid actorId)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(actorId, nameof(actorId));

        this.ChatId = chatId;
        this.ActorId = actorId;
    }

    public Guid ChatId { get; }
    public Guid ActorId { get; }
} 