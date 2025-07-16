using System;
using System.Collections.Generic;
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
    }

#nullable disable
    protected Chat()
    {
    }
#nullable restore

    public const int IdempotencyKeyMaxLength = 200;

    private readonly IList<User> users;

    public Guid CreatorId { get; }
    public IReadOnlyList<User> Users => this.users.AsReadOnly();
    public string IdempotencyKey { get; }

    protected void AddUser(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        if (!this.users.Contains(user))
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
}
