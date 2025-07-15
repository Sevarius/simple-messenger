using System;

namespace WebApi.Transfers;

public sealed record CreatePrivateChatTransfer
{
    public required Guid InterlocutorId { get; init; }
}
