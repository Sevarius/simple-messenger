using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Models;
using Client.SignalR;
using Client.Web;
using Client.Web.Models;
using EnsureThat;

namespace Client;

internal sealed class UserService : IAsyncDisposable
{
    public UserService(string signalRUrl, IWebClient webClient)
    {
        EnsureArg.IsNotNullOrWhiteSpace(signalRUrl, nameof(signalRUrl));
        EnsureArg.IsNotNull(webClient, nameof(webClient));

        this.webClient = webClient;
        this.signalRUrl = signalRUrl;
    }

    private readonly string signalRUrl;
    private readonly IWebClient webClient;
    private MessagesSignalRClient messagesSignalRClient = null!;
    private ChatModel currentChat = null!;
    private UserModel currentUser = null!;

    public async Task SignInAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));

        this.messagesSignalRClient = new MessagesSignalRClient(userId, this.signalRUrl);
        await this.messagesSignalRClient.ConnectAsync(cancellationToken);
        this.messagesSignalRClient.OnMessageReceived = this.MessageReceivedAsync;

        this.currentUser = await this.webClient.GetUserAsync(userId, cancellationToken);
    }

    public async Task SignOut(CancellationToken cancellationToken)
    {
        await this.messagesSignalRClient.DisconnectAsync(cancellationToken);
        await this.messagesSignalRClient.DisposeAsync();
        this.messagesSignalRClient = null!;
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
        this.currentChat = null!;
    }

    public async Task SendMessage(string message, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(message, nameof(message));

        await this.messagesSignalRClient.SendMessageAsync(this.currentChat.Id, message, cancellationToken);

        Console.WriteLine(message);
    }

    public async ValueTask DisposeAsync()
    {
        await this.messagesSignalRClient.DisposeAsync();
        await this.webClient.DisposeAsync();
    }

    private async Task MessageReceivedAsync(MessageModel message)
    {
        EnsureArg.IsNotNull(message, nameof(message));

        if (this.currentChat.Id == message.ChatId)
        {
            ConsoleWriter.WriteMessage(this.GetUser(message.ChatId), message);
        }
    }

    private UserModel GetUser(Guid userId) => this.currentChat.Users.Single(user => user.Id == userId);
}
