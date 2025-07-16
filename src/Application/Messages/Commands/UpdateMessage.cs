using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Messages.Commands;

public sealed record UpdateMessage : IRequest<MessageAndChatModel>
{
    public UpdateMessage(Guid actorId, Guid chatId, Guid messageId, string content)
    {
        EnsureArg.IsNotEmpty(actorId, nameof(actorId));
        EnsureArg.IsNotEmpty(chatId, nameof(chatId));
        EnsureArg.IsNotEmpty(messageId, nameof(messageId));
        EnsureArg.IsNotNullOrWhiteSpace(content, nameof(content));

        this.ActorId = actorId;
        this.ChatId = chatId;
        this.MessageId = messageId;
        this.Content = content;
    }

    public Guid ActorId { get; }
    public Guid ChatId { get; }
    public Guid MessageId { get; }
    public string Content { get; }
}
