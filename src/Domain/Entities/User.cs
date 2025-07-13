using System;
using EnsureThat;

namespace Domain.Entities;

public sealed class User : Entity
{
    public User(string userName)
        : base(Guid.NewGuid())
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));
        EnsureArg.IsLte(userName.Length, UserMaxLength, nameof(userName));

        this.UserName = userName;
    }

#nullable disable
    protected User()
    {
    }
#nullable restore

    public const int UserMaxLength = 100;

    public string UserName { get; }
    public bool IsDeleted { get; private set; }

    public void Delete()
    {
        this.IsDeleted = true;
    }
}
