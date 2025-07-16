using System;

namespace WebApi.Transfers;

public sealed record AddUserToGroupChatTransfer
{
    public required Guid UserId { get; init; }
} 