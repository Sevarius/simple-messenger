using System.Threading;
using System.Threading.Tasks;
using Application.Mappings;
using Application.Repositories;
using Application.Users.Commands;
using Domain.Entities;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

namespace Application.Users.CommandHandlers;

internal sealed class CreateUserHandler : IRequestHandler<CreateUser, UserModel>
{
    public CreateUserHandler(IUsersRepository usersRepository)
    {
        EnsureArg.IsNotNull(usersRepository, nameof(usersRepository));

        this.usersRepository = usersRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<CreateUserHandler>();
    private readonly IUsersRepository usersRepository;

    public async Task<UserModel> Handle(CreateUser command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        Logger.Information("Creating user with username {UserName}", command.UserName);

        var user = new User(command.UserName);

        this.usersRepository.Insert(user);

        await this.usersRepository.SaveChangesAsync(cancellationToken);

        Logger.Information("Successfully created user with ID {UserId} and username {UserName}", user.Id, user.UserName);

        return user.ToModel();
    }
}
