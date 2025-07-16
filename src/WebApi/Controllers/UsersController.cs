using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.Commands;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Models;
using Serilog;
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

    private static readonly ILogger Logger = Log.ForContext<UsersController>();
    private readonly IMediator mediator;

    [HttpPost]
    public async Task<UserModel> CreateUserAsync([FromBody] CreateUserTransfer transfer, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        Logger.Information("API: Creating user with username {UserName}", transfer.UserName);

        var result = await this.mediator.Send(new CreateUser(transfer.UserName), cancellationToken);

        Logger.Information("API: Successfully created user with ID {UserId}", result.Id);

        return result;
    }

    [HttpGet]
    public async Task<UserModel[]> ListUsersAsync(CancellationToken cancellationToken)
    {
        Logger.Information("API: Listing all users");

        var result = await this.mediator.Send(new ListUsers(), cancellationToken);

        Logger.Information("API: Successfully listed {UserCount} users", result.Length);

        return result;
    }

    [HttpGet("{userId}")]
    public async Task<UserModel> GetUserAsync(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        Logger.Information("API: Getting user with ID {UserId}", userId);

        var result = await this.mediator.Send(new GetUser(userId), cancellationToken);

        Logger.Information("API: Successfully retrieved user with ID {UserId}", userId);

        return result;
    }

    [HttpGet("{userId}/status")]
    public async Task<bool> GetUserStatus(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        Logger.Information("API: Checking if user with ID {UserId} is online", userId);

        var result = await this.mediator.Send(new GetUserStatus(userId), cancellationToken);

        Logger.Information("API: User with ID {UserId} is {OnlineStatus}", userId, result ? "online" : "offline");

        return result;
    }
}
