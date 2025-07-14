using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public Task<EntityCreatedResponse> CreateUserAsync(CreateUserRequest userRequest, CancellationToken cancellationToken = default)
    {
        EnsureArg.IsNotNull(userRequest, nameof(userRequest));

        return this.PostAsync<CreateUserRequest, EntityCreatedResponse>("api/users", userRequest, cancellationToken);
    }

    public async Task<UserResponse[]> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        var response = await this.httpClient.GetAsync("api/users", cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<UserResponse[]>(responseContent, this.jsonSettings)!;
    }

    public Task<EntityCreatedResponse> CreateChatAsync(CreatePrivateChatRequest request, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(request, nameof(request));

        return this.PostAsync<CreatePrivateChatRequest, EntityCreatedResponse>("api/chats", request, cancellationToken);
    }

    public Task<ChatResponse[]> ListChatsAsync(Guid userId, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotEmpty(userId, nameof(userId));

        return this.PostAsync<ChatResponse[]>($"api/chats?userId={userId}", cancellationToken);
    }

    private Task<TResponse> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken)
    {
        var json = JsonConvert.SerializeObject(request, this.jsonSettings);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return this.PostAsync<TResponse>(url, content, cancellationToken);
    }

    private Task<TResponse> PostAsync<TResponse>(string url, CancellationToken cancellationToken)
        => this.PostAsync<TResponse>(url, content: null, cancellationToken);

    private async Task<TResponse> PostAsync<TResponse>(string url, HttpContent? content, CancellationToken cancellationToken)
    {
        var response = await this.httpClient.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<TResponse>(responseContent, this.jsonSettings)!;
    }
}
