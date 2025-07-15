using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Application.Messages.Queries;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/chats/{chatId:guid}/messages")]
public class MessagesController : ControllerBase
{
    public MessagesController(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        this.mediator = mediator;
    }

    private readonly IMediator mediator;

    [HttpGet("{messageId:guid}")]
    public async Task<MessageResponse> GetMessageAsync(
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        var message = await this.mediator.Send(new GetMessage(chatId, messageId, this.ActorId), cancellationToken);

        return MessageResponse.From(message);
    }

    [HttpGet]
    public async Task<MessageResponse[]> ListMessagesAsync(
        [FromRoute] Guid chatId,
        CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(this.ActorId, nameof(this.ActorId));

        var messages = await this.mediator.Send(new ListMessages(chatId, this.ActorId), cancellationToken);

        return messages.Select(MessageResponse.From).ToArray();
    }

    private Guid ActorId => Guid.Parse(this.User!.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
}
