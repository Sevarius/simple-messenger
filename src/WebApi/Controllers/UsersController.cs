using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.Commands;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
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
    public async Task<EntityCreatedResponse> CreateUserAsync([FromBody] CreateUserTransfer transfer, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        var userId = await this.mediator.Send(new CreateUser(transfer.UserName), cancellationToken);

        return new EntityCreatedResponse
        {
            Id = userId,
        };
    }

    [HttpGet]
    public async Task<UserResponse[]> ListUsersAsync(CancellationToken cancellationToken)
    {
        var users = await this.mediator.Send(new ListUsers(), cancellationToken);

        return users.Select(UserResponse.From).ToArray();
    }

    [HttpGet("{userId}")]
    public async Task<UserResponse> GetUserAsync(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        var user = await this.mediator.Send(new GetUser(userId), cancellationToken);

        return UserResponse.From(user);
    }
}
