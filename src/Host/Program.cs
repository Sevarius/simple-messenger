using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using WebApi.Configuration;

namespace Host;

internal static class Program
{
    public static void Main(string[] args)
    {
        // Create initial configuration to bootstrap Serilog
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Configure Serilog from configuration
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Log.Information("Starting SimpleMessenger Host");
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Add Serilog
            builder.Host.UseSerilog();
            
            builder.Services.ConfigureWebApiServices();
            
            var app = builder.Build();

            app.UseWebApi();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}