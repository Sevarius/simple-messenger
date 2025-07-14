using System;
using System.Threading.Tasks;
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

        this.ConfigureEventHandlers();
    }

    private readonly HubConnection connection;
    private static readonly ILogger Logger = Log.ForContext<MessagesSignalRClient>();

    public event Action<Guid, Guid>? OnMessageReceived;

    public async Task ConnectAsync()
    {
        if (this.connection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await this.connection.StartAsync();
                Logger.Information("Connected to SignalR hub");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to connect to SignalR hub");
                throw;
            }
        }
    }

    public async Task DisconnectAsync()
    {
        if (this.connection.State == HubConnectionState.Connected)
        {
            try
            {
                await this.connection.StopAsync();
                Logger.Information("Disconnected from SignalR hub");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while disconnecting from SignalR hub");
                throw;
            }
        }
    }

    public async Task SendMessageAsync(Guid chatId, string content)
    {
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotNullOrWhiteSpace(content, nameof(content));

        if (this.connection.State != HubConnectionState.Connected)
        {
            throw new InvalidOperationException("SignalR connection is not established");
        }

        try
        {
            await this.connection.InvokeAsync("SendMessage", chatId, content);
            Logger.Information("Message sent to chat {ChatId}", chatId);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to send message to chat {ChatId}", chatId);
            throw;
        }
    }

    private void ConfigureEventHandlers()
    {
        this.connection.On<Guid, Guid>("ReceiveMessage", (messageId, chatId) =>
        {
            Logger.Information("Received message {MessageId} from chat {ChatId}", messageId, chatId);
            this.OnMessageReceived?.Invoke(messageId, chatId);
        });

        this.connection.Closed += error =>
        {
            if (error != null)
            {
                Logger.Error(error, "SignalR connection closed with error");
            }
            else
            {
                Logger.Information("SignalR connection closed");
            }

            return Task.CompletedTask;
        };

        this.connection.Reconnecting += error =>
        {
            Logger.Warning("SignalR connection reconnecting: {Error}", error?.Message);
            return Task.CompletedTask;
        };

        this.connection.Reconnected += connectionId =>
        {
            Logger.Information("SignalR connection reconnected with ID: {ConnectionId}", connectionId);
            return Task.CompletedTask;
        };
    }

    public async ValueTask DisposeAsync()
        => await this.connection.DisposeAsync();
}
