using System;
using EnsureThat;

namespace Domain.Entities;

public sealed class UserChatReadStatus : Entity
{
    public UserChatReadStatus(Guid userId, Guid chatId, DateTimeOffset lastReadMessageTimestamp)
        : base(Guid.NewGuid())
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(lastReadMessageTimestamp, nameof(lastReadMessageTimestamp));

        this.UserId = userId;
        this.ChatId = chatId;
        this.LastReadMessageTimestamp = lastReadMessageTimestamp;
    }

#nullable disable
    protected UserChatReadStatus()
    {
    }
#nullable enable

    public Guid UserId { get; }
    public Guid ChatId { get; }
    public DateTimeOffset LastReadMessageTimestamp { get; private set; }

    public bool UpdateLastReadMessageTimestamp(DateTimeOffset timestamp)
    {
        EnsureArg.IsNotDefault(timestamp, nameof(timestamp));
        EnsureArg.IsLte(timestamp, DateTimeOffset.UtcNow, nameof(timestamp));

        if (timestamp <= this.LastReadMessageTimestamp)
        {
            return false;
        }

        this.LastReadMessageTimestamp = timestamp;
        return true;
    }
}
