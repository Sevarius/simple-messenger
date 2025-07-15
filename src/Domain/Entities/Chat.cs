using System;
using System.Collections.Generic;
using EnsureThat;

namespace Domain.Entities;

public abstract class Chat : Entity
{
    protected Chat(User[] users)
        : base(Guid.NewGuid())
    {
        EnsureArg.IsNotNull(users, nameof(users));
        EnsureArg.HasItems(users, nameof(users));

        this.users = users;
    }

#nullable disable
    protected Chat()
    {
    }
#nullable restore

    private readonly IList<User> users;

    public IReadOnlyList<User> Users => this.users.AsReadOnly();

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

        if (this.users.Contains(user))
        {
            this.users.Remove(user);
        }
    }
}
