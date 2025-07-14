using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.Commands;
using Application.Users.Queries;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    public UsersController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        this.mediator = mediator;
    }

    private readonly IMediator mediator;

    [HttpPost]
    public async Task<EntityCreatedResponse> CreateUserAsync([FromBody] string name, CancellationToken cancellationToken)
    {
        var command = new CreateUser(name);

        var userId = await this.mediator.Send(command, cancellationToken);

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
}
