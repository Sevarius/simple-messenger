using System;
using System.Threading.Tasks;
using Client.SignalR;
using Client.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Client;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        var services = new ServiceCollection();

        ConfigureServices(configuration, services);

        var serviceProvider = services.BuildServiceProvider();

        var signalRClient = new MessagesSignalRClient(Guid.NewGuid(), configuration["SignalR:HubUrl"]!);
        await signalRClient.ConnectAsync();
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        var baseUrl = configuration["WebApi:BaseUrl"]!;

        // Configure HttpClient with base address
        services.AddHttpClient<IWebClient, WebClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });
    }
}
