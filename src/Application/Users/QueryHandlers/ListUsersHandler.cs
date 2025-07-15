using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Users.QueryHandlers;

internal sealed class ListUsersHandler : IRequestHandler<ListUsers, UserModel[]>
{
    public ListUsersHandler(IUsersReadOnlyRepository usersReadonlyRepository)
    {
        EnsureArg.IsNotNull(usersReadonlyRepository, nameof(usersReadonlyRepository));

        this.usersReadonlyRepository = usersReadonlyRepository;
    }

    private readonly IUsersReadOnlyRepository usersReadonlyRepository;

    public async Task<UserModel[]> Handle(ListUsers query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        return await this.usersReadonlyRepository.ListAsync(cancellationToken);
    }
}
