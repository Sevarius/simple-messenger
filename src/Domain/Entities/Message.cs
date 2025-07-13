using System;
using EnsureThat;

namespace Domain.Entities;

public sealed class Message : Entity
{
    public Message(Guid chatId, Guid userId, string content)
        : base(Guid.NewGuid())
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotNullOrWhiteSpace(content, nameof(content));
        EnsureArg.IsLte(content.Length, MessageMaxLength, nameof(content));

        this.ChatId = chatId;
        this.UserId = userId;
        this.Content = content;
        this.IsModified = false;
        this.IsDeleted = false;
        this.CreatedAt = DateTimeOffset.UtcNow;
    }

#nullable disable
    protected Message()
    {
    }
#nullable restore

    public const int MessageMaxLength = 1000;

    public Guid ChatId { get; }
    public Guid UserId { get; }
    public string Content { get; private set; }
    public bool IsModified { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public void Delete()
    {
        this.IsDeleted = true;
    }

    public void Modify(string newContent)
    {
        EnsureArg.IsNotNullOrWhiteSpace(newContent, nameof(newContent));
        EnsureArg.IsLte(newContent.Length, MessageMaxLength, nameof(newContent));

        if (this.IsDeleted)
        {
            throw new ApplicationException("The message has been deleted and cannot be modified.");
        }

        if (this.Content == newContent)
        {
            return;
        }

        this.Content = newContent;
        this.IsModified = true;
    }
}
