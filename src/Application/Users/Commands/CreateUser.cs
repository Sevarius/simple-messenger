using System;
using EnsureThat;
using MediatR;

namespace Application.Users.Commands;

public sealed record CreateUser : IRequest<Guid>
{
    public CreateUser(string userName)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName);

        this.UserName = userName;
    }

    public string UserName { get; }
}
