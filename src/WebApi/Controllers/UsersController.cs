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
    public Task<UserModel> CreateUserAsync([FromBody] CreateUserTransfer transfer, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        return this.mediator.Send(new CreateUser(transfer.UserName), cancellationToken);
    }

    [HttpGet]
    public Task<UserModel[]> ListUsersAsync(CancellationToken cancellationToken)
        => this.mediator.Send(new ListUsers(), cancellationToken);

    [HttpGet("{userId}")]
    public Task<UserModel> GetUserAsync(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        return this.mediator.Send(new GetUser(userId), cancellationToken);
    }
}
