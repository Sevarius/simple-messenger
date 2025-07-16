using Application.Services;
using EnsureThat;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Services.Cache;

internal sealed class UserStatusCacheService : IUserStatusService
{
    public UserStatusCacheService(IConnectionMultiplexer redisConnection)
    {
        EnsureArg.IsNotNull(redisConnection, nameof(redisConnection));

        this.redisDb = redisConnection.GetDatabase();
    }

    private readonly IDatabase redisDb;

    public async Task AddUserConnection(Guid userId, string connectionId)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));
        EnsureArg.IsNotEmpty(connectionId, nameof(connectionId));

        var userConnectionsKey = GetUserConnectionsKey(userId.ToString());
        await this.redisDb.SetAddAsync(userConnectionsKey, connectionId);
    }

    public async Task RemoveUserConnection(Guid userId, string connectionId)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));
        EnsureArg.IsNotEmpty(connectionId, nameof(connectionId));

        var userConnectionsKey = GetUserConnectionsKey(userId.ToString());
        await this.redisDb.SetRemoveAsync(userConnectionsKey, connectionId);
    }

    public async Task<bool> IsUserOnline(Guid userId)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));

        var userConnectionsKey = GetUserConnectionsKey(userId.ToString());
        return await this.redisDb.SetLengthAsync(userConnectionsKey) > 0;
    }

    private static string GetUserConnectionsKey(string userId) => $"user:{userId}:connections";
}
