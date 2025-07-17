using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Services;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApi.Hubs;

internal sealed class UserStatusHub : Hub
{
    public UserStatusHub(IUserStatusService userStatusService, IMediator mediator)
    {
        EnsureArg.IsNotNull(userStatusService, nameof(userStatusService));
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        this.userStatusService = userStatusService;
        this.mediator = mediator;
    }

    private readonly IUserStatusService userStatusService;
    private readonly IMediator mediator;

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId(this.Context);

        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId.ToString());

        await this.userStatusService.AddUserConnection(userId, this.Context.ConnectionId);

        var relatedUserIds = await this.mediator.Send(
            new ListRelatedUsers(userId),
            this.Context.ConnectionAborted);

        await this.Clients
            .Groups(relatedUserIds.Select(user => user.ToString()))
            .SendAsync("UserStatus", userId, true);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId(this.Context);

        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, userId.ToString());

        await this.userStatusService.RemoveUserConnection(userId, this.Context.ConnectionId);

        if (!await this.userStatusService.IsUserOnline(userId))
        {
            var relatedUserIds = await this.mediator.Send(new ListRelatedUsers(userId));

            await this.Clients
                .Groups(relatedUserIds.Select(user => user.ToString()))
                .SendAsync("UserStatus", userId, false);
        }
    }

    private static Guid GetUserId(HubCallerContext context)
        => Guid.Parse(context.User!.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
}
