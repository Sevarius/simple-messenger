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

    [HttpPost]
    public Task<ChatModel> CreateChatAsync(
        [FromBody] CreatePrivateChatTransfer transfer,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        return this.mediator.Send(new CreatePrivateChat(this.ActorId, transfer.InterlocutorId), cancellationToken);
    }

    [HttpGet]
    public Task<ChatModel[]> ListChatsAsync(CancellationToken cancellationToken)
        => this.mediator.Send(new ListChats(this.ActorId), cancellationToken);

    [HttpGet("{chatId}")]
    public Task<ChatModel> GetChatAsync(
        [FromRoute] Guid chatId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return this.mediator.Send(new GetChat(this.ActorId, chatId), cancellationToken);
    }

    private Guid ActorId => Guid.Parse(this.User!.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
}
