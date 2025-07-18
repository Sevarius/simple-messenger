using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Models;
using Client.SignalR;
using Client.Web;
using Client.Web.Models;
using EnsureThat;
using Serilog;

namespace Client;

internal sealed class UserService : IAsyncDisposable
{
    public UserService(SignalROptions signalROptions, IWebClient webClient)
    {
        EnsureArg.IsNotNull(signalROptions, nameof(signalROptions));
        EnsureArg.IsNotNull(webClient, nameof(webClient));

        this.webClient = webClient;
        this.signalROptions = signalROptions;
    }

    private static readonly ILogger Logger = Log.ForContext<UserService>();
    private readonly SignalROptions signalROptions;
    private readonly IWebClient webClient;
    private MessagesSignalRClient messagesSignalRClient = null!;
    private UserStatusSignalRClient userStatusSignalRClient = null!;
    private ChatModel currentChat = null!;
    private UserModel currentUser = null!;

    public async Task SignInAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));

        this.messagesSignalRClient = new MessagesSignalRClient(userId, this.signalROptions.MessagesHubUrl);
        await this.messagesSignalRClient.ConnectAsync(cancellationToken);
        this.messagesSignalRClient.OnMessageReceived = this.MessageReceivedAsync;
        this.messagesSignalRClient.OnMessageUpdated = this.MessageUpdatedAsync;
        this.messagesSignalRClient.OnMessageDeleted = this.MessageDeletedAsync;
        this.userStatusSignalRClient = new UserStatusSignalRClient(userId, this.signalROptions.UserStatusesHubUrl);
        await this.userStatusSignalRClient.ConnectAsync(cancellationToken);
        this.userStatusSignalRClient.OnUserStatusChanged = this.UserStatusChangedAsync;

        this.currentUser = await this.webClient.GetUserAsync(userId, cancellationToken);
    }

    public async Task SignOut(CancellationToken cancellationToken)
    {
        await this.messagesSignalRClient.DisconnectAsync(cancellationToken);
        await this.messagesSignalRClient.DisposeAsync();
        this.messagesSignalRClient = null!;
        await this.userStatusSignalRClient.DisconnectAsync(cancellationToken);
        await this.userStatusSignalRClient.DisposeAsync();
        this.userStatusSignalRClient = null!;
        this.currentUser = null!;
    }

    public async Task CreateChatAsync(Guid interlocutorId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(interlocutorId, nameof(interlocutorId));

        var chat = await this.webClient.CreatePrivateChatAsync(
            this.currentUser.Id,
            new CreatePrivateChatRequest
                {
                    InterlocutorId = interlocutorId
                },
            cancellationToken);

        ConsoleWriter.OpenChat(chat);

        this.currentChat = chat;
    }

    public async Task ListChats(CancellationToken cancellationToken)
    {
        var chats = await this.webClient.ListChatsAsync(this.currentUser.Id, cancellationToken);

        ConsoleWriter.ListChats(chats);
    }

    public async Task OpenChat(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(chatId, nameof(chatId));

        var chat = await this.webClient.GetChatAsync(this.currentUser.Id, chatId, cancellationToken);
        ConsoleWriter.OpenChat(chat);

        this.currentChat = chat;

        var messages = await this.webClient.ListMessagesAsync(this.currentUser.Id, chatId, cancellationToken);

        foreach (var message in messages)
        {
            ConsoleWriter.WriteMessage(this.GetUser(message.UserId), message);
        }
    }

    public async Task CloseChat(CancellationToken cancellationToken)
    {
        if (this.currentChat != null)
        {
            this.currentChat = null!;
        }
        else
        {
            Logger.Warning("Attempted to close chat but no chat is currently open");
        }
    }

    public async Task SendMessage(string message, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(message, nameof(message));

        if (this.currentChat == null)
        {
            Logger.Warning("Attempted to send message but no chat is open");
            return;
        }

        await this.messagesSignalRClient.SendMessageAsync(this.currentChat.Id, message, cancellationToken);

        Console.WriteLine(message);
    }

    public async Task UpdateMessage(Guid messageId, string newContent, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(messageId, nameof(messageId));
        EnsureArg.IsNotNullOrWhiteSpace(newContent, nameof(newContent));

        if (this.currentChat == null)
        {
            Logger.Warning("Attempted to update message but no chat is open");
            return;
        }

        await this.messagesSignalRClient.UpdateMessageAsync(this.currentChat.Id, messageId, newContent, cancellationToken);
    }

    public async Task DeleteMessage(Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        if (this.currentChat == null)
        {
            Logger.Warning("Attempted to delete message but no chat is open");
            return;
        }

        await this.messagesSignalRClient.DeleteMessageAsync(this.currentChat.Id, messageId, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await this.messagesSignalRClient.DisposeAsync();
        await this.webClient.DisposeAsync();

        Logger.Information("UserService disposed successfully");
    }

    private async Task MessageReceivedAsync(MessageModel message)
    {
        EnsureArg.IsNotNull(message, nameof(message));

        Logger.Information("Received message {MessageId} in chat {ChatId}", message.Id, message.ChatId);

        if (this.currentChat.Id == message.ChatId)
        {
            ConsoleWriter.WriteMessage(this.GetUser(message.UserId), message);
            Logger.Information("Message displayed for current chat {ChatId}", message.ChatId);
        }
        else
        {
            Logger.Information("Received message for different chat {MessageChatId}, current chat is {CurrentChatId}",
                message.ChatId, this.currentChat.Id);
        }
    }

    private async Task MessageUpdatedAsync(MessageModel message)
    {
        EnsureArg.IsNotNull(message, nameof(message));

        Logger.Information("Message {MessageId} updated in chat {ChatId}", message.Id, message.ChatId);

        if (this.currentChat.Id == message.ChatId)
        {
            Console.Write("[UPDATED] ");
            ConsoleWriter.WriteMessage(this.GetUser(message.UserId), message);
            Logger.Information("Updated message displayed for current chat {ChatId}", message.ChatId);
        }
        else
        {
            Logger.Information("Updated message for different chat {MessageChatId}, current chat is {CurrentChatId}",
                message.ChatId, this.currentChat.Id);
        }
    }

    private async Task MessageDeletedAsync(MessageModel message)
    {
        EnsureArg.IsNotNull(message, nameof(message));

        Logger.Information("Message {MessageId} deleted from chat {ChatId}", message.Id, message.ChatId);

        if (this.currentChat.Id == message.ChatId)
        {
            Console.Write("[DELETED] ");
            ConsoleWriter.WriteMessage(this.GetUser(message.UserId), message);
            Logger.Information("Deleted message displayed for current chat {ChatId}", message.ChatId);
        }
        else
        {
            Logger.Information("Deleted message for different chat {MessageChatId}, current chat is {CurrentChatId}",
                message.ChatId, this.currentChat.Id);
        }
    }

    private async Task UserStatusChangedAsync(Guid userId, bool isOnline)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        Logger.Information("User {UserId} is now {Status}", userId, isOnline ? "online" : "offline");

        ConsoleWriter.UserStatus(userId, isOnline);
    }

    private UserModel GetUser(Guid userId) => this.currentChat.Users.Single(user => user.Id == userId);
}
