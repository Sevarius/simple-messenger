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

        Logger.Information("SignalR: User {ActorId} sending message to chat {ChatId}", actorId, chatId);

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

        Logger.Information("SignalR: Successfully sent message {MessageId} to chat {ChatId} by user {ActorId}", messageAndChatModel.Message.Id, chatId, actorId);
    }

    public async Task UpdateMessage(Guid chatId, Guid messageId, string content)
    {
        var actorId = GetUserId(this.Context);

        Logger.Information("SignalR: User {ActorId} updating message {MessageId} in chat {ChatId}", actorId, messageId, chatId);

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

        Logger.Information("SignalR: Successfully updated message {MessageId} in chat {ChatId} by user {ActorId}", messageId, chatId, actorId);
    }

    public async Task DeleteMessage(Guid chatId, Guid messageId)
    {
        var actorId = GetUserId(this.Context);

        Logger.Information("SignalR: User {ActorId} deleting message {MessageId} in chat {ChatId}", actorId, messageId, chatId);

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

        Logger.Information("SignalR: Successfully deleted message {MessageId} in chat {ChatId} by user {ActorId}", messageId, chatId, actorId);
    }

    public async Task MarkMessagesAsRead(Guid chatId, DateTimeOffset lastReadMessageTimestamp)
    {
        var actorId = GetUserId(this.Context);

        Logger.Information("SignalR: User {ActorId} marking messages as read in chat {ChatId} up to message {LastReadMessageId}", actorId, chatId, lastReadMessageTimestamp);

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

        Logger.Information("SignalR: Successfully updated read status for user {ActorId} in chat {ChatId} up to message {LastReadMessageId}", actorId, chatId, lastReadMessageTimestamp);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId(this.Context);
        Logger.Information("SignalR: User {UserId} connected with connection {ConnectionId}", userId, this.Context.ConnectionId);

        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId.ToString());

        Logger.Information("SignalR: User {UserId} added to group", userId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId(this.Context);
        Logger.Information("SignalR: User {UserId} disconnected with connection {ConnectionId}", userId, this.Context.ConnectionId);

        if (exception != null)
        {
            Logger.Warning("SignalR: User {UserId} disconnected due to exception: {Exception}", userId, exception.Message);
        }

        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, userId.ToString());

        Logger.Information("SignalR: User {UserId} removed from group", userId);
    }

    private static Guid GetUserId(HubCallerContext context)
        => Guid.Parse(context.User!.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
}
