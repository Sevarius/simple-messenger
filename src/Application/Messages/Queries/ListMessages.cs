using System;
using System.Collections.Generic;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Messages.Queries;

public sealed record ListMessages : IRequest<IReadOnlyList<Message>>
{
    public ListMessages(Guid chatId, Guid actorId)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(actorId, nameof(actorId));

        this.ChatId = chatId;
        this.ActorId = actorId;
    }

    public Guid ChatId { get; }
    public Guid ActorId { get; }
} 