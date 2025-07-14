using System;
using System.Linq;
using Domain.Entities;
using EnsureThat;

namespace WebApi.Models;

public sealed record ChatResponse
{
    public required Guid Id { get; init; }
    public required UserResponse[] Users { get; init; }
    public required string ChatType { get; init; }

    public static ChatResponse From(Chat chat)
    {
        EnsureArg.IsNotNull(chat, nameof(chat));

        return new ChatResponse
        {
            Id = chat.Id,
            Users = chat.Users.Select(UserResponse.From).ToArray(),
            ChatType = chat.GetType().Name
        };
    }
} 