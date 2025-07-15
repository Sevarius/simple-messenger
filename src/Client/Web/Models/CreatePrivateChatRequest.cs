using System;

namespace Client.Web.Models;

public sealed record CreatePrivateChatRequest
{
    public required Guid InterlocutorId { get; init; }
}
