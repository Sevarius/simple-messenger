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

namespace Client.Web;

public sealed class WebClient : IWebClient
{
    public WebClient(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    private readonly HttpClient httpClient;
    private readonly JsonSerializerSettings jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc
    };

    // Users endpoints
    public Task<UserModel> CreateUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNullOrWhiteSpace(userName, nameof(userName));

        return this.PostAsync<CreateUserRequest, UserModel>(
            "api/users",
            new CreateUserRequest { UserName = userName },
            cancellationToken);
    }

    public Task<UserModel[]> ListUsersAsync(CancellationToken cancellationToken)
        => this.GetAsync<UserModel[]>("api/users", cancellationToken);

    public Task<UserModel> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(userId, nameof(userId));

        return this.GetAsync<UserModel>($"api/users/{userId}", cancellationToken);
    }

    // Chats endpoints
    public Task<ChatModel> CreatePrivateChatAsync(Guid actorId, CreatePrivateChatRequest request, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotNull(request, nameof(request));

        return this.PostAsync<CreatePrivateChatRequest, ChatModel>(actorId, "api/chats", request, cancellationToken);
    }

    public Task<ChatModel[]> ListChatsAsync(Guid actorId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));

        return this.GetAsync<ChatModel[]>(actorId, "api/chats", cancellationToken);
    }

    public Task<ChatModel> GetChatAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return this.GetAsync<ChatModel>(actorId, $"api/chats/{chatId}", cancellationToken);
    }

    // Messages endpoints
    public Task<MessageModel[]> ListMessagesAsync(Guid actorId, Guid chatId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));

        return this.GetAsync<MessageModel[]>(actorId, $"api/chats/{chatId}/messages", cancellationToken);
    }

    public Task<MessageModel> GetMessageAsync(Guid actorId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotDefault(chatId, nameof(chatId));
        EnsureArg.IsNotDefault(messageId, nameof(messageId));

        return this.GetAsync<MessageModel>(actorId, $"api/chats/{chatId}/messages/{messageId}", cancellationToken);
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

        var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;
    }

    private async Task<TResponse> GetAsync<TResponse>(string url, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNullOrWhiteSpace(url, nameof(url));

        var response = await this.httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;
    }

    private async Task<TResponse> GetAsync<TResponse>(Guid actorId, string url, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotDefault(actorId, nameof(actorId));
        EnsureArg.IsNotNullOrWhiteSpace(url, nameof(url));

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", actorId.ToString());

        var response = await this.httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;
    }

    public ValueTask DisposeAsync()
    {
        this.httpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
