using System;
using EnsureThat;
using MediatR;

namespace Application.Messages.Commands;

public sealed record CreateMessage : IRequest<Guid>
{
    public CreateMessage(Guid actorId, Guid chatId, string content)
    {
        EnsureArg.IsNotEmpty(actorId, nameof(actorId));
        EnsureArg.IsNotEmpty(chatId, nameof(actorId));
        EnsureArg.IsNotNullOrWhiteSpace(content, nameof(content));

        this.ActorId = actorId;
        this.ChatId = chatId;
        this.Content = content;
    }

    public Guid ActorId { get; }
    public Guid ChatId { get; }
    public string Content { get; }
}
