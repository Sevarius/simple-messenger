using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Chats.Queries;

public sealed record ListChats : IRequest<ChatModel[]>
{
    public ListChats(Guid userId)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        this.UserId = userId;
    }

    public Guid UserId { get; }
}
