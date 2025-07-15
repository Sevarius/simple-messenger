using System;

namespace Client.Models;

public sealed record UserModel
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
}
