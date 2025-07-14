using System;

namespace Client.Web.Models;

public sealed record CreatePrivateChatRequest
{
    public required Guid ActorId { get; init; }
    public required Guid InterlocutorId { get; init; }
}
