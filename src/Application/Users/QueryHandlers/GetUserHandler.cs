using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Users.QueryHandlers;

internal sealed class GetUserHandler : IRequestHandler<GetUser, UserModel>
{
    public GetUserHandler(IUsersReadOnlyRepository usersReadOnlyRepository)
    {
        EnsureArg.IsNotNull(usersReadOnlyRepository, nameof(usersReadOnlyRepository));

        this.usersReadOnlyRepository = usersReadOnlyRepository;
    }

    private readonly IUsersReadOnlyRepository usersReadOnlyRepository;

    public async Task<UserModel> Handle(GetUser query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var result = await this.usersReadOnlyRepository.GetAsync(query.UserId, cancellationToken);

        return result;
    }
}
