using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

namespace Application.Users.QueryHandlers;

internal sealed class ListUsersHandler : IRequestHandler<ListUsers, UserModel[]>
{
    public ListUsersHandler(IUsersReadOnlyRepository usersReadonlyRepository)
    {
        EnsureArg.IsNotNull(usersReadonlyRepository, nameof(usersReadonlyRepository));

        this.usersReadonlyRepository = usersReadonlyRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<ListUsersHandler>();
    private readonly IUsersReadOnlyRepository usersReadonlyRepository;

    public async Task<UserModel[]> Handle(ListUsers query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        Logger.Information("Listing all users");

        var users = await this.usersReadonlyRepository.ListAsync(cancellationToken);

        Logger.Information("Successfully retrieved {UserCount} users", users.Length);

        return users;
    }
}
