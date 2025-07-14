using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Messages.Commands;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace SignalRApi.Hubs;

[SignalRHub]
internal sealed class MessagesHub : Hub
{
    public MessagesHub(IMediator mediator)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        this.mediator = mediator;
    }

    private readonly IMediator mediator;

    public async Task SendMessage(Guid chatId, string content)
    {
        var userId = GetUserId(this.Context);

        var messageId = await this.mediator.Send(
                new CreateMessage(userId, chatId, content),
                this.Context.ConnectionAborted)
            .ConfigureAwait(false);

        await this.Clients.Group(userId.ToString())
            .SendAsync("ReceiveMessage", messageId, chatId)
            .ConfigureAwait(false);
    }

    public override async Task OnConnectedAsync()
        => await this.Groups.AddToGroupAsync(this.Context.ConnectionId, GetUserId(this.Context).ToString());

    public override async Task OnDisconnectedAsync(Exception? exception)
        => await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, GetUserId(this.Context).ToString());

    private static Guid GetUserId(HubCallerContext context)
        => Guid.Parse(context.User!.Claims.SingleOrDefault(claim => claim.Type == "user_id")?.Value!);
}
