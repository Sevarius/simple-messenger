using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace Domain.Entities;

public abstract class Chat : Entity
{
    protected Chat(User creator, User[] users, string idempotencyKey)
        : base(Guid.NewGuid())
    {
        EnsureArg.IsNotNull(creator, nameof(creator));
        EnsureArg.IsNotNull(users, nameof(users));
        EnsureArg.HasItems(users, nameof(users));
        EnsureArg.IsNotNullOrWhiteSpace(idempotencyKey, nameof(idempotencyKey));
        EnsureArg.IsLte(idempotencyKey.Length, IdempotencyKeyMaxLength, nameof(idempotencyKey));

        this.CreatorId = creator.Id;
        this.users = users;
        this.IdempotencyKey = idempotencyKey;
        this.CreatedAt = DateTimeOffset.Now;
        this.userChatReadStatuses = users.Select(user => new UserChatReadStatus(user.Id, this.Id, this.CreatedAt)).ToList();
        this.LastMessageTimestamp = this.CreatedAt;
    }

#nullable disable
    protected Chat()
    {
    }
#nullable restore

    public const int IdempotencyKeyMaxLength = 200;

    private readonly IList<User> users;
    private readonly IList<UserChatReadStatus> userChatReadStatuses;

    public Guid CreatorId { get; }
    public IReadOnlyList<User> Users => this.users.AsReadOnly();
    public IReadOnlyList<UserChatReadStatus> UserChatReadStatuses => this.userChatReadStatuses.AsReadOnly();
    public string IdempotencyKey { get; }
    public DateTimeOffset LastMessageTimestamp { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    protected void AddUser(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        if (this.Users.All(us => us.Id != user.Id))
        {
            this.users.Add(user);
        }
    }

    protected void RemoveUser(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        if (this.CreatorId == user.Id)
        {
            throw new InvalidOperationException("The creator of the chat cannot be removed.");
        }

        this.users.Remove(user);
    }

    public bool UpdateUserChatReadStatus(Guid userId, DateTimeOffset lastReadMessageTimestamp)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotDefault(lastReadMessageTimestamp, nameof(lastReadMessageTimestamp));

        if (this.Users.All(user => user.Id != userId))
        {
            throw new InvalidOperationException($"User {userId} is not part of chat {this.Id}");
        }

        var userChatReadStatus = this.UserChatReadStatuses.SingleOrDefault(status => status.UserId == userId);

        if (userChatReadStatus == null)
        {
            throw new InvalidOperationException($"No read status found for user {userId} in chat {this.Id}");
        }

        return userChatReadStatus.UpdateLastReadMessageTimestamp(lastReadMessageTimestamp);
    }

    public void SetLastMessageTimestamp(DateTimeOffset timestamp)
    {
        EnsureArg.IsNotDefault(timestamp, nameof(timestamp));
        EnsureArg.IsLte(timestamp, DateTimeOffset.UtcNow, nameof(timestamp));

        if (timestamp > this.LastMessageTimestamp)
        {
            this.LastMessageTimestamp = timestamp;
        }
    }
}
