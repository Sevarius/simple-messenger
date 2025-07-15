using System;
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
    }

    private readonly HubConnection connection;
    private static readonly ILogger Logger = Log.ForContext<MessagesSignalRClient>();

    public Func<MessageModel,Task> OnMessageReceived { get; set; } = (_) => Task.CompletedTask;

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

    public async ValueTask DisposeAsync()
        => await this.connection.DisposeAsync();
}
