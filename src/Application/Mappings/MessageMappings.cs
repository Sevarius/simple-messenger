using Domain.Entities;
using EnsureThat;
using Models;

namespace Application.Mappings;

public static class MessageMappings
{
    public static MessageModel ToModel(this Message message)
    {
        EnsureArg.IsNotNull(message, nameof(message));

        return new MessageModel
        {
            Id = message.Id,
            ChatId = message.ChatId,
            UserId = message.UserId,
            Content = message.Content,
            IsModified = message.IsModified,
            IsDeleted = message.IsDeleted,
            CreatedAt = message.CreatedAt,
        };
    }
}
