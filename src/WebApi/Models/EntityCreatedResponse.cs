using System;

namespace WebApi.Models;

public sealed record EntityCreatedResponse
{
    public required Guid Id { get; init; }
}
