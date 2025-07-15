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
    private static readonly ILogger Logger = Log.ForContext(typeof(Program));

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

        try
        {
            Logger.Information("Starting SimpleMessenger Client");

            var services = new ServiceCollection();

            ConfigureServices(configuration, services);

            var serviceProvider = services.BuildServiceProvider();

            var webClient = serviceProvider.GetRequiredService<IWebClient>();

            UserService = new UserService(configuration["SignalR:MessagesHubUrl"]!, webClient);

            Logger.Information("Client initialized successfully");

            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Enter command:");
                var command = Console.ReadLine();

                Logger.Information("User entered command: {Command}", command);

                switch (command)
                {
                    case "list users":
                        Logger.Information("Executing list users command");
                        var users = await webClient.ListUsersAsync(cancellationToken);
                        foreach (var user in users)
                        {
                            Console.WriteLine($"{user.Id} - {user.UserName}");
                        }
                        Logger.Information("Listed {UserCount} users", users.Length);
                        break;
                    case "create user":
                        Console.WriteLine("Enter user name:");
                        var userName = Console.ReadLine();
                        Logger.Information("Creating user with name: {UserName}", userName);
                        await webClient.CreateUserAsync(userName!, cancellationToken);
                        Logger.Information("User created successfully");
                        break;
                    case "sign in":
                        Console.WriteLine("Enter user id:");
                        var userId = Console.ReadLine();
                        Logger.Information("Signing in user: {UserId}", userId);
                        await UserService.SignInAsync(Guid.Parse(userId!), cancellationToken);
                        Logger.Information("User signed in successfully");
                        break;
                    case "sign out":
                        Logger.Information("Signing out user");
                        await UserService.SignOut(cancellationToken);
                        Logger.Information("User signed out successfully");
                        break;
                    case "create chat":
                        Console.WriteLine("Enter interlocutor id:");
                        var interlocutorId = Console.ReadLine();
                        Logger.Information("Creating chat with interlocutor: {InterlocutorId}", interlocutorId);
                        await UserService.CreateChatAsync(Guid.Parse(interlocutorId!), cancellationToken);
                        Logger.Information("Chat created successfully");
                        break;
                    case "list chats":
                        Logger.Information("Listing chats");
                        await UserService.ListChats(cancellationToken);
                        Logger.Information("Chats listed successfully");
                        break;
                    case "open chat":
                        Console.WriteLine("Enter chat id:");
                        var chatId = Console.ReadLine();
                        Logger.Information("Opening chat: {ChatId}", chatId);
                        await UserService.OpenChat(Guid.Parse(chatId!), cancellationToken);
                        Logger.Information("Chat opened successfully");
                        break;
                    case "send message":
                        Console.WriteLine("Enter message:");
                        var message = Console.ReadLine();
                        Logger.Information("Sending message");
                        await UserService.SendMessage(message!, cancellationToken);
                        Logger.Information("Message sent successfully");
                        break;
                    case "close chat":
                        Logger.Information("Closing chat");
                        await UserService.CloseChat(cancellationToken);
                        Logger.Information("Chat closed successfully");
                        break;
                    default:
                        Logger.Warning("Unknown command: {Command}", command);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Logger.Information("Shutting down SimpleMessenger Client");
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        var baseUrl = configuration["WebApi:BaseUrl"]!;

        Logger.Information("Configuring services with base URL: {BaseUrl}", baseUrl);

        // Configure HttpClient with base address
        services.AddHttpClient<IWebClient, WebClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        Logger.Information("Services configured successfully");
    }
}
