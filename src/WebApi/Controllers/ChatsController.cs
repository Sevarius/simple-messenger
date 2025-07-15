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
using WebApi.Models;
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
    public async Task<EntityCreatedResponse> CreateChatAsync(
        [FromBody] CreatePrivateChatTransfer transfer,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(transfer, nameof(transfer));

        var chatId = await this.mediator.Send(new CreatePrivateChat(this.ActorId, transfer.InterlocutorId), cancellationToken);

        return new EntityCreatedResponse
        {
            Id = chatId,
        };
    }

    [HttpGet]
    public async Task<ChatResponse[]> ListChatsAsync(CancellationToken cancellationToken)
    {
        var chats = await this.mediator.Send(new ListChats(this.ActorId), cancellationToken);

        return chats.Select(ChatResponse.From).ToArray();
    }

    [HttpGet("{chatId}")]
    public async Task<ChatResponse> GetChatAsync(
        [FromRoute] Guid chatId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        var chat = await this.mediator.Send(new GetChat(chatId, this.ActorId), cancellationToken);

        return ChatResponse.From(chat);
    }

    private Guid ActorId => Guid.Parse(this.User!.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
}
