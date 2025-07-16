using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Client.Models;
using EnsureThat;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace Client.SignalR;

internal sealed class MessagesSignalRClient : IAsyncDisposable
{
    public MessagesSignalRClient(Guid userId, string connectionString)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));
        EnsureArg.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));

        this.connection = new HubConnectionBuilder()
            .WithUrl(connectionString,
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(userId.ToString())!;
                })
            .WithAutomaticReconnect()
            .Build();

        this.connection.On<MessageModel>("ReceiveMessage", async (message) =>
        {
            await this.OnMessageReceived.Invoke(message);
        });

        this.connection.On<MessageModel>("UpdateMessage", async (message) =>
        {
            await this.OnMessageUpdated.Invoke(message);
        });

        this.connection.On<MessageModel>("DeleteMessage", async (message) =>
        {
            await this.OnMessageDeleted.Invoke(message);
        });
    }

    private readonly HubConnection connection;
    private static readonly ILogger Logger = Log.ForContext<MessagesSignalRClient>();

    public Func<MessageModel,Task> OnMessageReceived { get; set; } = _ => Task.CompletedTask;
    public Func<MessageModel,Task> OnMessageUpdated { get; set; } = _ => Task.CompletedTask;
    public Func<MessageModel,Task> OnMessageDeleted { get; set; } = _ => Task.CompletedTask;

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        if (this.connection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await this.connection.StartAsync(cancellationToken);
                Logger.Information("Connected to SignalR hub");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to connect to SignalR hub");
                throw;
            }
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken)
    {
        if (this.connection.State == HubConnectionState.Connected)
        {
            try
            {
                await this.connection.StopAsync(cancellationToken);
                Logger.Information("Disconnected from SignalR hub");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while disconnecting from SignalR hub");
                throw;
            }
        }
    }

    public async Task SendMessageAsync(Guid chatId, string content, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotNullOrWhiteSpace(content, nameof(content));

        try
        {
            await this.connection.InvokeAsync(
                "SendMessage",
                chatId,
                content,
                cancellationToken: cancellationToken);
            Logger.Information("Message sent to chat {ChatId}", chatId);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to send message to chat {ChatId}", chatId);
            throw;
        }
    }

    public async Task UpdateMessageAsync(Guid chatId, Guid messageId, string content, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));
        EnsureArg.IsNotNullOrWhiteSpace(content, nameof(content));

        try
        {
            await this.connection.InvokeAsync(
                "UpdateMessage",
                chatId,
                messageId,
                content,
                cancellationToken: cancellationToken);
            Logger.Information("Message {MessageId} updated in chat {ChatId}", messageId, chatId);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to update message {MessageId} in chat {ChatId}", messageId, chatId);
            throw;
        }
    }

    public async Task DeleteMessageAsync(Guid chatId, Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        try
        {
            await this.connection.InvokeAsync(
                "DeleteMessage",
                chatId,
                messageId,
                cancellationToken: cancellationToken);
            Logger.Information("Message {MessageId} deleted from chat {ChatId}", messageId, chatId);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to delete message {MessageId} from chat {ChatId}", messageId, chatId);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
        => await this.connection.DisposeAsync();
}
