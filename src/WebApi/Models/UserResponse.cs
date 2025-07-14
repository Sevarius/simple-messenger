using System;
using Domain.Entities;
using EnsureThat;

namespace WebApi.Models;

public sealed record UserResponse
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }

    public static UserResponse From(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        return new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName
        };
    }
}
