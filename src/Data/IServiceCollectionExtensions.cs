using System.Reflection;
using Application.Repositories;
using Data.Repositories;
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

        services.AddScoped<IChatsReadOnlyRepository, ChatsReadOnlyRepository>();
        services.AddScoped<IChatsRepository, ChatsRepository>();
        services.AddScoped<IMessagesReadOnlyRepository, MessagesReadOnlyRepository>();
        services.AddScoped<IMessagesRepository, MessagesRepository>();
        services.AddScoped<IUsersReadOnlyRepository, UsersReadOnlyRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();

        return services;
    }
}
