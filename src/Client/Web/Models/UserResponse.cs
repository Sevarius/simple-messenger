using System;

namespace Client.Web.Models;

public sealed record UserResponse
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
}
