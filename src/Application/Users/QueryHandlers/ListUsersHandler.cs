using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Users.Queries;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Users.QueryHandlers;

internal sealed class ListUsersHandler : IRequestHandler<ListUsers, IReadOnlyList<User>>
{
    public ListUsersHandler(IUsersReadOnlyRepository usersReadonlyRepository)
    {
        EnsureArg.IsNotNull(usersReadonlyRepository, nameof(usersReadonlyRepository));

        this.usersReadonlyRepository = usersReadonlyRepository;
    }

    private readonly IUsersReadOnlyRepository usersReadonlyRepository;

    public async Task<IReadOnlyList<User>> Handle(ListUsers query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        return await this.usersReadonlyRepository.ListAsync(cancellationToken);
    }
}
