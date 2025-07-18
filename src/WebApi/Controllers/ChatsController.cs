using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Commands;
using Application.Chats.Queries;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Models;
using WebApi.Transfers;

namespace WebApi.Controllers;

[ApiController]
[Route("api/chats")]
public class ChatsController : ControllerBase
{
    public ChatsController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        this.mediator = mediator;
    }

    private readonly IMediator mediator;

    [HttpPost("private")]
    public async Task<ChatModel> CreatePrivateChatAsync(
        [FromBody] CreatePrivateChatTransfer transfer,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        var result = await this.mediator.Send(new CreatePrivateChat(this.ActorId, transfer.InterlocutorId), cancellationToken);

        return result;
    }

    [HttpPost("group")]
    public async Task<ChatModel> CreateGroupChatAsync(
        [FromBody] CreateGroupChatTransfer transfer,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        var result = await this.mediator.Send(new CreateGroupChat(this.ActorId, transfer.Name), cancellationToken);

        return result;
    }

    [HttpPost("{chatId}/users")]
    public async Task<IActionResult> AddUserToGroupChatAsync(
        [FromRoute] Guid chatId,
        [FromBody] AddUserToGroupChatTransfer transfer,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        await this.mediator.Send(new AddUserToGroupChat(this.ActorId, chatId, transfer.UserId), cancellationToken);

        return this.Ok();
    }

    [HttpDelete("{chatId}/users/{userId}")]
    public async Task<IActionResult> RemoveUserFromGroupChatAsync(
        [FromRoute] Guid chatId,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(userId, nameof(userId));

        await this.mediator.Send(new RemoveUserFromGroupChat(this.ActorId, chatId, userId), cancellationToken);

        return this.Ok();
    }

    [HttpGet]
    public async Task<ChatModel[]> ListChatsAsync(CancellationToken cancellationToken)
    {
        var result = await this.mediator.Send(new ListChats(this.ActorId), cancellationToken);

        return result;
    }

    [HttpGet("{chatId}")]
    public async Task<ChatModel> GetChatAsync(
        [FromRoute] Guid chatId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        var result = await this.mediator.Send(new GetChat(this.ActorId, chatId), cancellationToken);

        return result;
    }

    private Guid ActorId => Guid.Parse(this.User!.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
}
