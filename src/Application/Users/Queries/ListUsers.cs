using System.Collections.Generic;
using Domain.Entities;
using MediatR;

namespace Application.Users.Queries;

public sealed record ListUsers : IRequest<IReadOnlyList<User>>;
