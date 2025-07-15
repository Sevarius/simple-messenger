using System;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Users.Queries;

public sealed record GetUser : IRequest<UserModel>
{
    public GetUser(Guid userId)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        this.UserId = userId;
    }

    public Guid UserId { get; }
}
