using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories;

public interface IUsersReadOnlyRepository
{
    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken);

    Task<User> GetAsync(Guid userId, CancellationToken cancellationToken);
}
