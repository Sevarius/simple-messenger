using System;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace Client.SignalR;

internal sealed class UserStatusSignalRClient : IAsyncDisposable
{
    public UserStatusSignalRClient(Guid userId, string connectionString)
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

        this.connection.On<Guid, bool>("UserStatus", async (usId, isOnline) =>
        {
            await this.OnUserStatusChanged.Invoke(usId, isOnline);
        });
    }

    private readonly HubConnection connection;
    private static readonly ILogger Logger = Log.ForContext<UserStatusSignalRClient>();

    public Func<Guid, bool, Task> OnUserStatusChanged { get; set; } = (_, _) => Task.CompletedTask;

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        if (this.connection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await this.connection.StartAsync(cancellationToken);
                Logger.Information("Connected to UserStatus SignalR hub");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to connect to UserStatus SignalR hub");
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
                Logger.Information("Disconnected from UserStatus SignalR hub");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while disconnecting from UserStatus SignalR hub");
                throw;
            }
        }
    }

    public async ValueTask DisposeAsync()
        => await this.connection.DisposeAsync();
}
