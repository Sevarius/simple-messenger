using System;

namespace Client.Web.Models;

public sealed class MessageResponse
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsModified { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
} 