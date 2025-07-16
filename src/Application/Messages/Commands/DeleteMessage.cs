using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Messages.Commands;

public sealed record DeleteMessage : IRequest<MessageAndChatModel>
{
    public DeleteMessage(Guid actorId, Guid chatId, Guid messageId)
    {
        EnsureArg.IsNotEmpty(actorId, nameof(actorId));
        EnsureArg.IsNotEmpty(chatId, nameof(chatId));
        EnsureArg.IsNotEmpty(messageId, nameof(messageId));

        this.ActorId = actorId;
        this.ChatId = chatId;
        this.MessageId = messageId;
    }

    public Guid ActorId { get; }
    public Guid ChatId { get; }
    public Guid MessageId { get; }
}
