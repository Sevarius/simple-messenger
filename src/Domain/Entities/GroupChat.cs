using EnsureThat;

namespace Domain.Entities;

public sealed class GroupChat : Chat
{
    public GroupChat(User creator, string name)
        : base(creator, [creator], $"{creator.Id}-{name}")
    {
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
        EnsureArg.IsLte(name.Length, GroupChatNameMaxLength, nameof(name));

        this.Name = name;
    }

#nullable disable
    protected GroupChat()
    {
    }
#nullable restore

    public const int GroupChatNameMaxLength = 100;
    public string Name { get; private set; }

    public new void AddUser(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        base.AddUser(user);
    }

    public new void RemoveUser(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        base.RemoveUser(user);
    }
}
