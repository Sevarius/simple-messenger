using System;
using System.Threading.Tasks;

namespace Application.Services;

public interface IUserStatusService
{
    Task AddUserConnection(Guid userId, string connectionId);
    Task RemoveUserConnection(Guid userId, string connectionId);
    Task<bool> IsUserOnline(Guid userId);
}
