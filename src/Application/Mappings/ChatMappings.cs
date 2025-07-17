using System.Linq;
using Domain.Entities;
using EnsureThat;
using Models;

namespace Application.Mappings;

public static class ChatMappings
{
    public static ChatModel ToModel(this Chat chat)
    {
        EnsureArg.IsNotNull(chat, nameof(chat));

        return new ChatModel
        {
            Id = chat.Id,
            CreatorId = chat.CreatorId,
            Users = chat.Users.Select(user => user.ToModel()).ToArray(),
            UserChatReadStatuses = chat.UserChatReadStatuses.Select(status => status.ToModel()).ToArray(),
            Name = chat is GroupChat groupChat ? groupChat.Name : null,
        };
    }

    public static UserChatReadStatusModel ToModel(this UserChatReadStatus status)
    {
        EnsureArg.IsNotNull(status, nameof(status));

        return new UserChatReadStatusModel
        {
            UserId = status.UserId,
            ChatId = status.ChatId,
            LastReadMessageTimestamp = status.LastReadMessageTimestamp,
        };
    }
}
