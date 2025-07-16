using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.Users.Queries;
using EnsureThat;
using MediatR;

namespace Application.Users.QueryHandlers;

internal sealed class GetUserStatusHandler : IRequestHandler<GetUserStatus, bool>
{
    public GetUserStatusHandler(IUserStatusService userStatusService)
    {
        EnsureArg.IsNotNull(userStatusService, nameof(userStatusService));

        this.userStatusService = userStatusService;
    }

    private readonly IUserStatusService userStatusService;

    public async Task<bool> Handle(GetUserStatus query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        return await this.userStatusService.IsUserOnline(query.UserId);
    }
}
