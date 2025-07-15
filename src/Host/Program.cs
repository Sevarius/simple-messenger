using System;
using System.IO;
using System.Linq;
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
using SignalRApi;
using SignalRApi.Configuration;

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
            Logger.Information("Configuration loaded from {BasePath}", Directory.GetCurrentDirectory());

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            Logger.Information("Configuring services");
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            Logger.Information("Mapping controllers and SignalR hubs");
            app.MapControllers();
            app.MapSignalRHubs();

            if (app.Environment.IsDevelopment())
            {
                Logger.Information("Development environment detected, enabling Swagger");
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleMessenger API V1");
                    c.RoutePrefix = "swagger";
                });
                Logger.Information("Swagger UI available at /swagger");
            }
            else
            {
                Logger.Information("Production environment detected, Swagger disabled");
            }

            Logger.Information("SimpleMessenger Host started successfully");
            app.Run();
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
        Logger.Information("Configuring MediatR");
        services.AddMediatR(
            configure =>
            {
                configure.RegisterServicesFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
            });

        Logger.Information("Configuring ASP.NET Core services");
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSignalR();

        Logger.Information("Configuring authentication with CustomToken scheme");
        services.AddAuthentication("CustomToken")
            .AddScheme<BearerTokenOptions, CustomTokenSchemeHandler>("CustomToken", configureOptions: null);

        var connectionString = configuration.GetConnectionString("DataBase")!;
        Logger.Information("Configuring database context with connection string: {DatabaseConnection}", 
            connectionString.Replace(connectionString.Split(';').FirstOrDefault(x => x.Contains("Password"))?.Split('=')[1] ?? "", "***"));

        services.AddDbContext(connectionString, typeof(MigrationsAssemblyReference).Assembly);

        Logger.Information("Configuring Swagger documentation");
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

        Logger.Information("Service configuration completed successfully");
    }
}
