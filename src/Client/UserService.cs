using System;
using System.Threading;
using System.Threading.Tasks;
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
    private Guid currentChatId = Guid.Empty;
    private Guid currentUserId = Guid.Empty;

    public async Task SignInAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));

        this.messagesSignalRClient = new MessagesSignalRClient(userId, this.signalRUrl);
        await this.messagesSignalRClient.ConnectAsync(cancellationToken);
        this.messagesSignalRClient.OnMessageReceived = this.MessageReceivedAsync;
        this.currentUserId = userId;
    }

    public async Task SignOut(CancellationToken cancellationToken)
    {
        await this.messagesSignalRClient.DisconnectAsync(cancellationToken);
        await this.messagesSignalRClient.DisposeAsync();
        this.messagesSignalRClient = null!;
    }

    public async Task CreateChatAsync(Guid interlocutorId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(interlocutorId, nameof(interlocutorId));

        var chatId = await this.webClient.CreatePrivateChatAsync(
            this.currentUserId,
            new CreatePrivateChatRequest
                {
                    InterlocutorId = interlocutorId
                },
            cancellationToken);

        var chat = await this.webClient.GetChatAsync(chatId.Id, this.currentUserId, cancellationToken);
        ConsoleWriter.OpenChat(chat);

        this.currentChatId = chat.Id;
    }

    public async Task ListChats(CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(this.currentChatId, nameof(this.currentChatId));

        var chats = await this.webClient.ListChatsAsync(this.currentUserId, cancellationToken);

        ConsoleWriter.ListChats(chats);
    }

    public async Task OpenChat(Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(chatId, nameof(chatId));

        var chat = await this.webClient.GetChatAsync(chatId, this.currentUserId, cancellationToken);
        ConsoleWriter.OpenChat(chat);

        var messages = await this.webClient.ListMessagesAsync(this.currentUserId, chatId, cancellationToken);
        ConsoleWriter.ListMessages(messages);

        this.currentChatId = chatId;
    }

    public async Task CloseChat(CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(this.currentChatId, nameof(this.currentChatId));

        this.currentChatId = Guid.Empty;
    }

    public async Task SendMessage(string message, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(message, nameof(message));

        await this.messagesSignalRClient.SendMessageAsync(this.currentChatId, message, cancellationToken);

        Console.WriteLine(message);
    }

    public async ValueTask DisposeAsync()
    {
        await this.messagesSignalRClient.DisposeAsync();
        await this.webClient.DisposeAsync();
    }

    public async Task MessageReceivedAsync(Guid messageId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(messageId, nameof(messageId));
        EnsureArg.IsNotEmpty(chatId, nameof(chatId));

        if (this.currentChatId == chatId)
        {
            var message = await this.webClient.GetMessageAsync(this.currentUserId, this.currentChatId, messageId, cancellationToken);

            ConsoleWriter.WriteMessage(message);
        }
    }
}
