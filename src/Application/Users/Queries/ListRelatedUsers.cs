using System;
using EnsureThat;
using MediatR;

namespace Application.Users.Queries;

public sealed record ListRelatedUsers : IRequest<Guid[]>
{
    public ListRelatedUsers(Guid actorId)
    {
        EnsureArg.IsNotEmpty(actorId, nameof(actorId));

        this.ActorId = actorId;
    }

    public Guid ActorId { get; }
}
