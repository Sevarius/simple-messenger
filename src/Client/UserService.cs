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
    public UserService(string signalRUrl, IWebClient webClient)
    {
        EnsureArg.IsNotNullOrWhiteSpace(signalRUrl, nameof(signalRUrl));
        EnsureArg.IsNotNull(webClient, nameof(webClient));

        this.webClient = webClient;
        this.signalRUrl = signalRUrl;

        Logger.Information("UserService initialized with SignalR URL: {SignalRUrl}", signalRUrl);
    }

    private static readonly ILogger Logger = Log.ForContext<UserService>();
    private readonly string signalRUrl;
    private readonly IWebClient webClient;
    private MessagesSignalRClient messagesSignalRClient = null!;
    private ChatModel currentChat = null!;
    private UserModel currentUser = null!;

    public async Task SignInAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));

        Logger.Information("Signing in user {UserId}", userId);

        this.messagesSignalRClient = new MessagesSignalRClient(userId, this.signalRUrl);
        await this.messagesSignalRClient.ConnectAsync(cancellationToken);
        this.messagesSignalRClient.OnMessageReceived = this.MessageReceivedAsync;

        this.currentUser = await this.webClient.GetUserAsync(userId, cancellationToken);

        Logger.Information("User {UserId} ({UserName}) signed in successfully", userId, this.currentUser.UserName);
    }

    public async Task SignOut(CancellationToken cancellationToken)
    {
        if (this.currentUser != null)
        {
            Logger.Information("Signing out user {UserId} ({UserName})", this.currentUser.Id, this.currentUser.UserName);
        }

        await this.messagesSignalRClient.DisconnectAsync(cancellationToken);
        await this.messagesSignalRClient.DisposeAsync();
        this.messagesSignalRClient = null!;
        this.currentUser = null!;

        Logger.Information("User signed out successfully");
    }

    public async Task CreateChatAsync(Guid interlocutorId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(interlocutorId, nameof(interlocutorId));

        Logger.Information("Creating private chat between user {UserId} and interlocutor {InterlocutorId}", 
            this.currentUser.Id, interlocutorId);

        var chat = await this.webClient.CreatePrivateChatAsync(
            this.currentUser.Id,
            new CreatePrivateChatRequest
                {
                    InterlocutorId = interlocutorId
                },
            cancellationToken);

        ConsoleWriter.OpenChat(chat);

        this.currentChat = chat;

        Logger.Information("Private chat {ChatId} created successfully", chat.Id);
    }

    public async Task ListChats(CancellationToken cancellationToken)
    {
        Logger.Information("Listing chats for user {UserId}", this.currentUser.Id);

        var chats = await this.webClient.ListChatsAsync(this.currentUser.Id, cancellationToken);

        ConsoleWriter.ListChats(chats);

        Logger.Information("Listed {ChatCount} chats for user {UserId}", chats.Length, this.currentUser.Id);
    }

    public async Task OpenChat(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(chatId, nameof(chatId));

        Logger.Information("Opening chat {ChatId} for user {UserId}", chatId, this.currentUser.Id);

        var chat = await this.webClient.GetChatAsync(this.currentUser.Id, chatId, cancellationToken);
        ConsoleWriter.OpenChat(chat);

        this.currentChat = chat;

        var messages = await this.webClient.ListMessagesAsync(this.currentUser.Id, chatId, cancellationToken);

        Logger.Information("Chat {ChatId} opened with {MessageCount} messages", chatId, messages.Length);

        foreach (var message in messages)
        {
            ConsoleWriter.WriteMessage(this.GetUser(message.UserId), message);
        }
    }

    public async Task CloseChat(CancellationToken cancellationToken)
    {
        if (this.currentChat != null)
        {
            Logger.Information("Closing chat {ChatId}", this.currentChat.Id);
            this.currentChat = null!;
            Logger.Information("Chat closed successfully");
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

        Logger.Information("Sending message to chat {ChatId}", this.currentChat.Id);

        await this.messagesSignalRClient.SendMessageAsync(this.currentChat.Id, message, cancellationToken);

        Console.WriteLine(message);

        Logger.Information("Message sent to chat {ChatId} successfully", this.currentChat.Id);
    }

    public async ValueTask DisposeAsync()
    {
        Logger.Information("Disposing UserService");

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
            ConsoleWriter.WriteMessage(this.GetUser(message.ChatId), message);
            Logger.Information("Message displayed for current chat {ChatId}", message.ChatId);
        }
        else
        {
            Logger.Information("Received message for different chat {MessageChatId}, current chat is {CurrentChatId}", 
                message.ChatId, this.currentChat.Id);
        }
    }

    private UserModel GetUser(Guid userId) => this.currentChat.Users.Single(user => user.Id == userId);
}
