using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

namespace Application.Users.QueryHandlers;

internal sealed class GetUserHandler : IRequestHandler<GetUser, UserModel>
{
    public GetUserHandler(IUsersReadOnlyRepository usersReadOnlyRepository)
    {
        EnsureArg.IsNotNull(usersReadOnlyRepository, nameof(usersReadOnlyRepository));

        this.usersReadOnlyRepository = usersReadOnlyRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<GetUserHandler>();
    private readonly IUsersReadOnlyRepository usersReadOnlyRepository;

    public async Task<UserModel> Handle(GetUser query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        Logger.Information("Getting user with ID {UserId}", query.UserId);

        var result = await this.usersReadOnlyRepository.GetAsync(query.UserId, cancellationToken);

        Logger.Information("Successfully retrieved user with ID {UserId}", query.UserId);

        return result;
    }
}
