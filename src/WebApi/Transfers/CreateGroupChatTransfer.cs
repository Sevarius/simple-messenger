namespace WebApi.Transfers;

public sealed record CreateGroupChatTransfer
{
    public required string Name { get; init; }
}
