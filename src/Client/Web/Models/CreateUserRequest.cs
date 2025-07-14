namespace Client.Web.Models;

public sealed record CreateUserRequest
{
    public required string UserName { get; init; }
}
