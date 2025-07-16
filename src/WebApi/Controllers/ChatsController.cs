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
using Serilog;
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

    private static readonly ILogger Logger = Log.ForContext<ChatsController>();
    private readonly IMediator mediator;

    [HttpPost("private")]
    public async Task<ChatModel> CreatePrivateChatAsync(
        [FromBody] CreatePrivateChatTransfer transfer,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        Logger.Information("API: Creating private chat between actor {ActorId} and interlocutor {InterlocutorId}", this.ActorId, transfer.InterlocutorId);

        var result = await this.mediator.Send(new CreatePrivateChat(this.ActorId, transfer.InterlocutorId), cancellationToken);

        Logger.Information("API: Successfully created private chat with ID {ChatId}", result.Id);

        return result;
    }

    [HttpPost("group")]
    public async Task<ChatModel> CreateGroupChatAsync(
        [FromBody] CreateGroupChatTransfer transfer,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        Logger.Information("API: Creating group chat with name {Name} by actor {ActorId}", transfer.Name, this.ActorId);

        var result = await this.mediator.Send(new CreateGroupChat(this.ActorId, transfer.Name), cancellationToken);

        Logger.Information("API: Successfully created group chat with ID {ChatId} and name {Name}", result.Id, transfer.Name);

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

        Logger.Information("API: Adding user {UserId} to group chat {ChatId} by actor {ActorId}", transfer.UserId, chatId, this.ActorId);

        await this.mediator.Send(new AddUserToGroupChat(this.ActorId, chatId, transfer.UserId), cancellationToken);

        Logger.Information("API: Successfully added user {UserId} to group chat {ChatId}", transfer.UserId, chatId);

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

        Logger.Information("API: Removing user {UserId} from group chat {ChatId} by actor {ActorId}", userId, chatId, this.ActorId);

        await this.mediator.Send(new RemoveUserFromGroupChat(this.ActorId, chatId, userId), cancellationToken);

        Logger.Information("API: Successfully removed user {UserId} from group chat {ChatId}", userId, chatId);

        return this.Ok();
    }

    [HttpGet]
    public async Task<ChatModel[]> ListChatsAsync(CancellationToken cancellationToken)
    {
        Logger.Information("API: Listing chats for user {ActorId}", this.ActorId);

        var result = await this.mediator.Send(new ListChats(this.ActorId), cancellationToken);

        Logger.Information("API: Successfully listed {ChatCount} chats for user {ActorId}", result.Length, this.ActorId);

        return result;
    }

    [HttpGet("{chatId}")]
    public async Task<ChatModel> GetChatAsync(
        [FromRoute] Guid chatId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        Logger.Information("API: Getting chat {ChatId} for user {ActorId}", chatId, this.ActorId);

        var result = await this.mediator.Send(new GetChat(this.ActorId, chatId), cancellationToken);

        Logger.Information("API: Successfully retrieved chat {ChatId} for user {ActorId}", chatId, this.ActorId);

        return result;
    }

    private Guid ActorId => Guid.Parse(this.User!.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
}
