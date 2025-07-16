using System;

namespace WebApi.Transfers;

public sealed record RemoveUserFromGroupChatTransfer
{
    public required Guid UserId { get; init; }
} 