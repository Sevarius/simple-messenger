using System;

namespace Client.Web.Models;

public sealed record ChatResponse
{
    public required Guid Id { get; init; }
    public required UserResponse[] Users { get; init; }
    public required string ChatType { get; init; }
}
