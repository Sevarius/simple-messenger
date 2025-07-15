using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Models;
using Client.Web.Models;
using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Client.Web;

public sealed class WebClient : IWebClient
{
    public WebClient(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Logger.Information("WebClient initialized with base address: {BaseAddress}", httpClient.BaseAddress);
    }

    private static readonly ILogger Logger = Log.ForContext<WebClient>();
    private readonly HttpClient httpClient;
    private readonly JsonSerializerSettings jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc
    };

    // Users endpoints
    public async Task<UserModel> CreateUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));

        Logger.Information("Creating user with name: {UserName}", userName);

        var result = await this.PostAsync<CreateUserRequest, UserModel>(
            "api/users",
            new CreateUserRequest { UserName = userName },
            cancellationToken);

        Logger.Information("User created successfully with ID: {UserId}", result.Id);
        return result;
    }

    public async Task<UserModel[]> ListUsersAsync(CancellationToken cancellationToken)
    {
        Logger.Information("Listing all users");

        var result = await this.GetAsync<UserModel[]>("api/users", cancellationToken);

        Logger.Information("Successfully retrieved {UserCount} users", result.Length);
        return result;
    }

    public async Task<UserModel> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        Logger.Information("Getting user with ID: {UserId}", userId);

        var result = await this.GetAsync<UserModel>($"api/users/{userId}", cancellationToken);

        Logger.Information("Successfully retrieved user {UserId} ({UserName})", result.Id, result.UserName);
        return result;
    }

    // Chats endpoints
    public async Task<ChatModel> CreatePrivateChatAsync(Guid actorId, CreatePrivateChatRequest request, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotNull(request, nameof(request));

        Logger.Information("Creating private chat for actor {ActorId} with interlocutor {InterlocutorId}", 
            actorId, request.InterlocutorId);

        var result = await this.PostAsync<CreatePrivateChatRequest, ChatModel>(actorId, "api/chats", request, cancellationToken);

        Logger.Information("Private chat created successfully with ID: {ChatId}", result.Id);
        return result;
    }

    public async Task<ChatModel[]> ListChatsAsync(Guid actorId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));

        Logger.Information("Listing chats for user {ActorId}", actorId);

        var result = await this.GetAsync<ChatModel[]>(actorId, "api/chats", cancellationToken);

        Logger.Information("Successfully retrieved {ChatCount} chats for user {ActorId}", result.Length, actorId);
        return result;
    }

    public async Task<ChatModel> GetChatAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        Logger.Information("Getting chat {ChatId} for user {ActorId}", chatId, actorId);

        var result = await this.GetAsync<ChatModel>(actorId, $"api/chats/{chatId}", cancellationToken);

        Logger.Information("Successfully retrieved chat {ChatId} for user {ActorId}", chatId, actorId);
        return result;
    }

    // Messages endpoints
    public async Task<MessageModel[]> ListMessagesAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        Logger.Information("Listing messages for chat {ChatId} by user {ActorId}", chatId, actorId);

        var result = await this.GetAsync<MessageModel[]>(actorId, $"api/chats/{chatId}/messages", cancellationToken);

        Logger.Information("Successfully retrieved {MessageCount} messages for chat {ChatId}", result.Length, chatId);
        return result;
    }

    public async Task<MessageModel> GetMessageAsync(Guid actorId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        Logger.Information("Getting message {MessageId} from chat {ChatId} for user {ActorId}", messageId, chatId, actorId);

        var result = await this.GetAsync<MessageModel>(actorId, $"api/chats/{chatId}/messages/{messageId}", cancellationToken);

        Logger.Information("Successfully retrieved message {MessageId} from chat {ChatId}", messageId, chatId);
        return result;
    }

    // Helper methods
    private Task<TResponse> PostAsync<TRequest, TResponse>(
        Guid actorId,
        string url,
        TRequest request,
        CancellationToken cancellationToken)
    {
        var json = JsonConvert.SerializeObject(request, this.jsonSettings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return this.PostAsync<TResponse>(actorId, url, content, cancellationToken);
    }

    private Task<TResponse> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken)
    {
        var json = JsonConvert.SerializeObject(request, this.jsonSettings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return this.PostAsync<TResponse>(actorId: null, url, content, cancellationToken);
    }

    private async Task<TResponse> PostAsync<TResponse>(Guid? actorId, string url, HttpContent content, CancellationToken cancellationToken)
    {
        Logger.Information("Making POST request to {Url} with actor {ActorId}", url, actorId);

        var request = new HttpRequestMessage(HttpMethod.Post, url);

        if (actorId is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", actorId.ToString());
        }

        request.Content = content;

        try
        {
            var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;

            Logger.Information("POST request to {Url} completed successfully", url);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "POST request to {Url} failed", url);
            throw;
        }
    }

    private async Task<TResponse> GetAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(url, nameof(url));

        Logger.Information("Making GET request to {Url}", url);

        try
        {
            var response = await this.httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;

            Logger.Information("GET request to {Url} completed successfully", url);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "GET request to {Url} failed", url);
            throw;
        }
    }

    private async Task<TResponse> GetAsync<TResponse>(Guid actorId, string url, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotNullOrWhiteSpace(url, nameof(url));

        Logger.Information("Making authenticated GET request to {Url} for actor {ActorId}", url, actorId);

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", actorId.ToString());

        try
        {
            var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;

            Logger.Information("Authenticated GET request to {Url} completed successfully", url);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Authenticated GET request to {Url} failed for actor {ActorId}", url, actorId);
            throw;
        }
    }

    public ValueTask DisposeAsync()
    {
        Logger.Information("Disposing WebClient");
        this.httpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
