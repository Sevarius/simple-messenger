using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Chats.Commands;

public sealed record CreateGroupChat : IRequest<ChatModel>
{
    public CreateGroupChat(Guid creatorId, string name)
    {
        EnsureArg.IsNotEmpty(creatorId, nameof(creatorId));
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

        this.CreatorId = creatorId;
        this.Name = name;
    }

    public Guid CreatorId { get; }
    public string Name { get; }
} 