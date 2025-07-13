using System;
using EnsureThat;

namespace Domain.Entities;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        EnsureArg.IsNotDefault(id, nameof(id));

        this.Id = id;
    }

#nullable disable
    protected Entity()
    {
    }
#nullable restore

    public Guid Id { get; }
}
