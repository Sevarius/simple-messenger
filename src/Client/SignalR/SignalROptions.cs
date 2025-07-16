namespace Client.SignalR;

public sealed record SignalROptions
{
    public string MessagesHubUrl { get; set; }
    public string UserStatusesHubUrl { get; set; }
}
