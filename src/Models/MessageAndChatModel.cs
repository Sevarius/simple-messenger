namespace Models;

public sealed record MessageAndChatModel
{
    public required MessageModel Message { get; init; }
    public required ChatModel Chat { get; init; }
}
