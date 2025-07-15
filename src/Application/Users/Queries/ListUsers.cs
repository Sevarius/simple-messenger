using MediatR;
using Models;

namespace Application.Users.Queries;

public sealed record ListUsers : IRequest<UserModel[]>;
