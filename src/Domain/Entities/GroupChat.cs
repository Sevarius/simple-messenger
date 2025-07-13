using System;
using EnsureThat;

namespace Domain.Entities;

public sealed class GroupChat : Chat
{
    public GroupChat(User creator, string name)
        : base([creator])
    {
        EnsureArg.IsNotNull(creator, nameof(creator));
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
        EnsureArg.IsLte(name.Length, GroupChatMaxLength, nameof(name));

        this.CreatorId = creator.Id;
        this.Name = name;
    }

#nullable disable
    protected GroupChat()
    {
    }
#nullable restore

    public const int GroupChatMaxLength = 100;

    public Guid CreatorId { get; }
    public string Name { get; private set; }

    public void ChangeName(string newName, Guid actorId)
    {
        EnsureArg.IsNotNullOrWhiteSpace(newName, nameof(newName));
        EnsureArg.IsLte(newName.Length, GroupChatMaxLength, nameof(newName));

        if (this.CreatorId != actorId)
        {
            throw new ApplicationException("Only the creator of the group chat can change its name.");
        }

        if (this.Name == newName)
        {
            return;
        }

        this.Name = newName;
    }
}
