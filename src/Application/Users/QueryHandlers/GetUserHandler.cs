using System.Threading;
using System.Threading.Tasks;
using Application.Repositories;
using Application.Users.Queries;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Users.QueryHandlers;

internal sealed class GetUserHandler : IRequestHandler<GetUser, User>
{
    public GetUserHandler(IUsersReadOnlyRepository usersReadOnlyRepository)
    {
        EnsureArg.IsNotNull(usersReadOnlyRepository, nameof(usersReadOnlyRepository));

        this.usersReadOnlyRepository = usersReadOnlyRepository;
    }

    private readonly IUsersReadOnlyRepository usersReadOnlyRepository;

    public async Task<User> Handle(GetUser query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        return await this.usersReadOnlyRepository.GetAsync(query.UserId, cancellationToken);
    }
} 