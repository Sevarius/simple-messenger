using System;
using Domain.Entities;

namespace WebApi.Models;

public sealed class MessageResponse
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsModified { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public static MessageResponse From(Message message)
    {
        return new MessageResponse
        {
            Id = message.Id,
            ChatId = message.ChatId,
            UserId = message.UserId,
            Content = message.Content,
            IsModified = message.IsModified,
            CreatedAt = message.CreatedAt
        };
    }
} 