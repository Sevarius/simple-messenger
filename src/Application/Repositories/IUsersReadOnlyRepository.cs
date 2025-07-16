using System;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Application.Repositories;

public interface IUsersReadOnlyRepository
{
    Task<UserModel[]> ListAsync(CancellationToken cancellationToken);

    Task<UserModel> GetAsync(Guid userId, CancellationToken cancellationToken);

    Task<UserModel[]> ListRelatedUsersAsync(Guid actorId, CancellationToken cancellationToken);
}
