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

        var result = await this.PostAsync<CreateUserRequest, UserModel>(
            "api/users",
            new CreateUserRequest { UserName = userName },
            cancellationToken);

        return result;
    }

    public async Task<UserModel[]> ListUsersAsync(CancellationToken cancellationToken)
    {
        var result = await this.GetAsync<UserModel[]>("api/users", cancellationToken);

        return result;
    }

    public async Task<UserModel> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        var result = await this.GetAsync<UserModel>($"api/users/{userId}", cancellationToken);

        return result;
    }

    // Chats endpoints
    public async Task<ChatModel> CreatePrivateChatAsync(Guid actorId, CreatePrivateChatRequest request, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotNull(request, nameof(request));

        var result = await this.PostAsync<CreatePrivateChatRequest, ChatModel>(actorId, "api/chats", request, cancellationToken);

        return result;
    }

    public async Task<ChatModel[]> ListChatsAsync(Guid actorId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));

        var result = await this.GetAsync<ChatModel[]>(actorId, "api/chats", cancellationToken);

        return result;
    }

    public async Task<ChatModel> GetChatAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        var result = await this.GetAsync<ChatModel>(actorId, $"api/chats/{chatId}", cancellationToken);

        return result;
    }

    // Messages endpoints
    public async Task<MessageModel[]> ListMessagesAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        var result = await this.GetAsync<MessageModel[]>(actorId, $"api/chats/{chatId}/messages", cancellationToken);

        return result;
    }

    public async Task<MessageModel> GetMessageAsync(Guid actorId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        var result = await this.GetAsync<MessageModel>(actorId, $"api/chats/{chatId}/messages/{messageId}", cancellationToken);

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
        var request = new HttpRequestMessage(HttpMethod.Post, url);

        if (actorId is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", actorId.ToString());
        }

        request.Content = content;

        try
        {
            var response = await this.httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;

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

        try
        {
            var response = await this.httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;

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

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", actorId.ToString());

        try
        {
            var response = await this.httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;

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
        this.httpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
