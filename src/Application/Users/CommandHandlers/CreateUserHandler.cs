using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Users.Commands;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Users.CommandHandlers;

internal sealed class CreateUserHandler : IRequestHandler<CreateUser, Guid>
{
    public CreateUserHandler(IUsersRepository usersRepository)
    {
        EnsureArg.IsNotNull(usersRepository, nameof(usersRepository));

        this.usersRepository = usersRepository;
    }

    private readonly IUsersRepository usersRepository;

    public async Task<Guid> Handle(CreateUser command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var user = new User(command.UserName);

        this.usersRepository.Insert(user);

        await this.usersRepository.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
