using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Chats.Commands;

public sealed record UpdateUserChatReadStatus : IRequest<ChatModel>
{
    public UpdateUserChatReadStatus(Guid userId, Guid chatId, DateTimeOffset lastReadMessageTimestamp)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(lastReadMessageTimestamp, nameof(lastReadMessageTimestamp));

        this.UserId = userId;
        this.ChatId = chatId;
        this.LastReadMessageTimestamp = lastReadMessageTimestamp;
    }

    public Guid UserId { get; }
    public Guid ChatId { get; }
    public DateTimeOffset LastReadMessageTimestamp { get; }
}
