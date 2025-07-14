using System;
using System.Collections.Generic;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Chats.Queries;

public sealed record ListChats : IRequest<IReadOnlyList<Chat>>
{
    public ListChats(Guid userId)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        this.UserId = userId;
    }

    public Guid UserId { get; }
} 