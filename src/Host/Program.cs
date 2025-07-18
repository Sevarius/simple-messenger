using System;
using System.IO;
using Application;
using Data;
using Data.Migrations;
using Host.Authentification;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Services.DependencyInjection;
using SignalRApi;
using SignalRApi.Configuration;
using StackExchange.Redis;

namespace Host;

internal static class Program
{
    private static readonly ILogger Logger = Log.ForContext(typeof(Program));

    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Logger.Information("Starting SimpleMessenger Host");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            app.MapControllers();
            app.MapSignalRHubs();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleMessenger API V1");
                        c.RoutePrefix = "swagger";
                    });
            }

            Logger.Information("SimpleMessenger Host started successfully");
            app.Run();
        }
        catch (HostAbortedException)
        {
            // Occurs during ef core migrations
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Logger.Information("Shutting down SimpleMessenger Host");
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(
            configure =>
            {
                configure.RegisterServicesFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
            });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSignalR();

        services.AddAuthentication("CustomToken")
            .AddScheme<BearerTokenOptions, CustomTokenSchemeHandler>("CustomToken", configureOptions: null);

        var connectionString = configuration.GetConnectionString("DataBase")!;

        services.AddDbContext(connectionString, typeof(MigrationsAssemblyReference).Assembly);

        services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "SimpleMessenger API"
            });

            swaggerGenOptions.AddSignalRSwaggerGen(signalRSwaggerGenOptions =>
                signalRSwaggerGenOptions.ScanAssemblies(typeof(SignalRAssemblyReference).Assembly));
        });

        // Configure Redis connection
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));

        services.AddStatusService();
    }
}
