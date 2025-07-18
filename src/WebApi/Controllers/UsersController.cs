using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.Commands;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApi.Transfers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    public UsersController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        this.mediator = mediator;
    }

    private readonly IMediator mediator;

    [HttpPost]
    public async Task<UserModel> CreateUserAsync([FromBody] CreateUserTransfer transfer, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        var result = await this.mediator.Send(new CreateUser(transfer.UserName), cancellationToken);

        return result;
    }

    [HttpGet]
    public async Task<UserModel[]> ListUsersAsync(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new ListUsers(), cancellationToken);

        return result;
    }

    [HttpGet("{userId}")]
    public async Task<UserModel> GetUserAsync(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        var result = await this.mediator.Send(new GetUser(userId), cancellationToken);

        return result;
    }

    [HttpGet("{userId}/status")]
    public async Task<bool> GetUserStatus(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        var result = await this.mediator.Send(new GetUserStatus(userId), cancellationToken);

        return result;
    }
}
