using System;

namespace Client.Web.Models;

public sealed record EntityCreatedResponse
{
    public required Guid Id { get; init; }
}
