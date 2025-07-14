using System;
using System.Linq;
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
[Route("api/[controller]")]
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

        var chatId = await this.mediator.Send(new CreatePrivateChat(transfer.ActorId, transfer.InterlocutorId), cancellationToken);

        return new EntityCreatedResponse
        {
            Id = chatId,
        };
    }

    [HttpGet]
    public async Task<ChatResponse[]> ListChatsAsync(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        var chats = await this.mediator.Send(new ListChats(userId), cancellationToken);

        return chats.Select(ChatResponse.From).ToArray();
    }
}
