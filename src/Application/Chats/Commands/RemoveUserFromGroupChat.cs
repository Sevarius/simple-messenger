using System;
using EnsureThat;
using MediatR;

namespace Application.Chats.Commands;

public sealed record RemoveUserFromGroupChat : IRequest
{
    public RemoveUserFromGroupChat(Guid actorId, Guid chatId, Guid userId)
    {
        EnsureArg.IsNotEmpty(actorId, nameof(actorId));
        EnsureArg.IsNotEmpty(chatId, nameof(chatId));
        EnsureArg.IsNotEmpty(userId, nameof(userId));

        this.ActorId = actorId;
        this.ChatId = chatId;
        this.UserId = userId;
    }

    public Guid ActorId { get; }
    public Guid ChatId { get; }
    public Guid UserId { get; }
} 