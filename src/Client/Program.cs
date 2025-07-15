using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Client;

internal static class Program
{
    private static UserService UserService = null!;

    private static async Task Main(string[] args)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, _) => cancellationTokenSource.Cancel();
        var cancellationToken = cancellationTokenSource.Token;

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

        var webClient = serviceProvider.GetRequiredService<IWebClient>();

        UserService = new UserService(configuration["SignalR:MessagesHubUrl"]!, webClient);

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Enter command:");
            var command = Console.ReadLine();

            switch (command)
            {
                case "list users":
                    var users = await webClient.ListUsersAsync(cancellationToken);
                    foreach (var user in users)
                    {
                        Console.WriteLine($"{user.Id} - {user.UserName}");
                    }
                    break;
                case "create user":
                    Console.WriteLine("Enter user name:");
                    var userName = Console.ReadLine();
                    await webClient.CreateUserAsync(userName!, cancellationToken);
                    break;
                case "sign in":
                    Console.WriteLine("Enter user id:");
                    var userId = Console.ReadLine();
                    await UserService.SignInAsync(Guid.Parse(userId!), cancellationToken);
                    break;
                case "sign out":
                    await UserService.SignOut(cancellationToken);
                    break;
                case "create chat":
                    Console.WriteLine("Enter interlocutor id:");
                    var interlocutorId = Console.ReadLine();
                    await UserService.CreateChatAsync(Guid.Parse(interlocutorId!), cancellationToken);
                    break;
                case "list chats":
                    await UserService.ListChats(cancellationToken);
                    break;
                case "open chat":
                    Console.WriteLine("Enter chat id:");
                    var chatId = Console.ReadLine();
                    await UserService.OpenChat(Guid.Parse(chatId!), cancellationToken);
                    break;
                case "send message":
                    Console.WriteLine("Enter message:");
                    var message = Console.ReadLine();
                    await UserService.SendMessage(message!, cancellationToken);
                    break;
                case "close chat":
                    await UserService.CloseChat(cancellationToken);
                    break;
            }
        }
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
