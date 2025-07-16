using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Users.Queries;
using EnsureThat;
using MediatR;

namespace Application.Users.QueryHandlers;

internal sealed class ListRelatedUsersHandler : IRequestHandler<ListRelatedUsers, Guid[]>
{
    public ListRelatedUsersHandler(IUsersReadOnlyRepository usersReadOnlyRepository)
    {
        EnsureArg.IsNotNull(usersReadOnlyRepository, nameof(usersReadOnlyRepository));

        this.usersReadOnlyRepository = usersReadOnlyRepository;
    }

    private readonly IUsersReadOnlyRepository usersReadOnlyRepository;

    public async Task<Guid[]> Handle(ListRelatedUsers query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        var relatedUsers = await this.usersReadOnlyRepository.ListRelatedUsersAsync(query.ActorId, cancellationToken);

        return relatedUsers.Select(user => user.Id).ToArray();
    }
}
