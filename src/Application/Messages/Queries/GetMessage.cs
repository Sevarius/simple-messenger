using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Messages.Queries;

public sealed record GetMessage : IRequest<MessageModel>
{
    public GetMessage(Guid actorId, Guid chatId, Guid messageId)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        this.ActorId = actorId;
        this.ChatId = chatId;
        this.MessageId = messageId;
    }

    public Guid ActorId { get; }
    public Guid ChatId { get; }
    public Guid MessageId { get; }
}
