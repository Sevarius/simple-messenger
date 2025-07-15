using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Chats.Commands;

public sealed record CreatePrivateChat : IRequest<ChatModel>
{
    public CreatePrivateChat(Guid actorId, Guid interlocutorId)
    {
        EnsureArg.IsNotEmpty(actorId, nameof(actorId));
        EnsureArg.IsNotEmpty(interlocutorId, nameof(interlocutorId));

        this.ActorId = actorId;
        this.InterlocutorId = interlocutorId;
    }

    public Guid ActorId { get; }
    public Guid InterlocutorId { get; }
}
