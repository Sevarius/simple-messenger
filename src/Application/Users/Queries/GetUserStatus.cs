using System;
using EnsureThat;
using MediatR;

namespace Application.Users.Queries;

public sealed class GetUserStatus : IRequest<bool>
{
    public GetUserStatus(Guid userId)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));

        this.UserId = userId;
    }

    public Guid UserId { get; }
}
