using System;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Messages.Queries;

public sealed record GetMessage : IRequest<Message>
{
    public GetMessage(Guid chatId, Guid messageId, Guid actorId)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));
        EnsureArg.IsNotDefault(actorId, nameof(actorId));

        this.ChatId = chatId;
        this.MessageId = messageId;
        this.ActorId = actorId;
    }

    public Guid ChatId { get; }
    public Guid MessageId { get; }
    public Guid ActorId { get; }
}
