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

    [HttpPost]
    public async Task<ChatModel> CreateChatAsync(
        [FromBody] CreatePrivateChatTransfer transfer,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        Logger.Information("API: Creating private chat between actor {ActorId} and interlocutor {InterlocutorId}", this.ActorId, transfer.InterlocutorId);

        var result = await this.mediator.Send(new CreatePrivateChat(this.ActorId, transfer.InterlocutorId), cancellationToken);

        Logger.Information("API: Successfully created private chat with ID {ChatId}", result.Id);

        return result;
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
