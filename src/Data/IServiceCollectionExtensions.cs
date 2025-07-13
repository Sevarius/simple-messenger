using System.Reflection;
using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        string connectionString,
        Assembly migrationAssembly)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));
        EnsureArg.IsNotNull(migrationAssembly, nameof(migrationAssembly));

        services.AddDbContext<MessengerDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure();

                    sqlServerOptions.MigrationsAssembly(migrationAssembly.GetName().Name);
                });

            options.UseSnakeCaseNamingConvention();
        });

        return services;
    }
}
