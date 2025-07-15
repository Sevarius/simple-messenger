using System;

namespace Client.Models;

public sealed class MessageModel
{
    public required Guid Id { get; set; }
    public required Guid ChatId { get; set; }
    public required Guid UserId { get; set; }
    public required string Content { get; set; }
    public required bool IsModified { get; set; }
    public required bool IsDeleted { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
