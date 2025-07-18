using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Chats.Commands;
using Application.Messages.Commands;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Serilog;
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

    private static readonly ILogger Logger = Log.ForContext<MessagesHub>();
    private readonly IMediator mediator;

    public async Task SendMessage(Guid chatId, string content)
    {
        var actorId = GetUserId(this.Context);

        var messageAndChatModel = await this.mediator.Send(
                new CreateMessage(actorId, chatId, content),
                this.Context.ConnectionAborted);

        var usersToNotify = messageAndChatModel.Chat.Users
            .Where(user => user.Id != actorId)
            .Select(user => user.Id.ToString())
            .ToList();

        await this.Clients
            .Groups(usersToNotify.Select(user => user.ToString()))
            .SendAsync("ReceiveMessage", messageAndChatModel.Message);
    }

    public async Task UpdateMessage(Guid chatId, Guid messageId, string content)
    {
        var actorId = GetUserId(this.Context);

        var messageAndChatModel = await this.mediator.Send(
                new UpdateMessage(actorId, chatId, messageId, content),
                this.Context.ConnectionAborted);

        var usersToNotify = messageAndChatModel.Chat.Users
            .Where(user => user.Id != actorId)
            .Select(user => user.Id.ToString())
            .ToList();

        await this.Clients
            .Groups(usersToNotify.Select(user => user.ToString()))
            .SendAsync("UpdateMessage", messageAndChatModel.Message);
    }

    public async Task DeleteMessage(Guid chatId, Guid messageId)
    {
        var actorId = GetUserId(this.Context);

        var messageAndChatModel = await this.mediator.Send(
                new DeleteMessage(actorId, chatId, messageId),
                this.Context.ConnectionAborted);

        var usersToNotify = messageAndChatModel.Chat.Users
            .Where(user => user.Id != actorId)
            .Select(user => user.Id.ToString())
            .ToList();

        await this.Clients
            .Groups(usersToNotify.Select(user => user.ToString()))
            .SendAsync("DeleteMessage", messageAndChatModel.Message);
    }

    public async Task MarkMessagesAsRead(Guid chatId, DateTimeOffset lastReadMessageTimestamp)
    {
        var actorId = GetUserId(this.Context);

        var chat = await this.mediator.Send(
            new UpdateUserChatReadStatus(actorId, chatId, lastReadMessageTimestamp),
            this.Context.ConnectionAborted);

        var usersToNotify = chat.Users
            .Where(user => user.Id != actorId)
            .Select(user => user.Id.ToString())
            .ToList();

        await this.Clients
            .Groups(usersToNotify)
            .SendAsync("UpdateUserReadStatus", actorId, chatId, lastReadMessageTimestamp);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId(this.Context);

        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId.ToString());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId(this.Context);

        if (exception != null)
        {
            Logger.Warning("SignalR: User {UserId} disconnected due to exception: {Exception}", userId, exception.Message);
        }

        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, userId.ToString());
    }

    private static Guid GetUserId(HubCallerContext context)
        => Guid.Parse(context.User!.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
}
