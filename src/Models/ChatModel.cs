using System;

namespace Models;

public sealed record ChatModel
{
    public required Guid Id { get; init; }
    public required UserModel[] Users { get; init; }
    public required string? Name { get; init; }
}
