using EnsureThat;
using MediatR;
using Models;

namespace Application.Users.Commands;

public sealed record CreateUser : IRequest<UserModel>
{
    public CreateUser(string userName)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName);

        this.UserName = userName;
    }

    public string UserName { get; }
}
