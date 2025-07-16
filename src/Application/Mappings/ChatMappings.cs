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
            Name = chat is GroupChat groupChat ? groupChat.Name : null,
        };
    }
}
