using Domain.Entities;
using EnsureThat;
using Models;

namespace Application.Mappings;

public static class UserMappings
{
    public static UserModel ToModel(this User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        return new UserModel
        {
            Id = user.Id,
            UserName = user.UserName
        };
    }
}
