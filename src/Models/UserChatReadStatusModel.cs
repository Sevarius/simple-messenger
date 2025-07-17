using System;

namespace Models;

public sealed record UserChatReadStatusModel
{
    public required Guid UserId { get; init; }
    public required Guid ChatId { get; init; }
    public required DateTimeOffset LastReadMessageTimestamp { get; init; }
}
