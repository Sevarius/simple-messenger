using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories;

public interface IUsersRepository
{
    Task<User> GetAsync(Guid userId, CancellationToken cancellationToken);

    Task<User?> FindAsync(Guid userId, CancellationToken cancellationToken);

    Task<User[]> ListAsync(CancellationToken cancellationToken);

    void Insert(User user);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
