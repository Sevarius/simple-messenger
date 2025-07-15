using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Messages.Commands;

public sealed record CreateMessage : IRequest<MessageModel>
{
    public CreateMessage(Guid actorId, Guid chatId, string content)
    {
        EnsureArg.IsNotEmpty(actorId, nameof(actorId));
        EnsureArg.IsNotEmpty(chatId, nameof(chatId));
        EnsureArg.IsNotNullOrWhiteSpace(content, nameof(content));

        this.ActorId = actorId;
        this.ChatId = chatId;
        this.Content = content;
    }

    public Guid ActorId { get; }
    public Guid ChatId { get; }
    public string Content { get; }
}
