using System;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Users.Queries;

public sealed record GetUser : IRequest<User>
{
    public GetUser(Guid userId)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        this.UserId = userId;
    }

    public Guid UserId { get; }
} 