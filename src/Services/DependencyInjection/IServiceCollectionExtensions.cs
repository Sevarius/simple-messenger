using Application.Services;
using EnsureThat;
using Microsoft.Extensions.DependencyInjection;
using Services.Cache;

namespace Services.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddStatusService(this IServiceCollection serviceCollection)
    {
        EnsureArg.IsNotNull(serviceCollection, nameof(serviceCollection));

        serviceCollection.AddScoped<IUserStatusService, UserStatusCacheService>();

        return serviceCollection;
    }
}
