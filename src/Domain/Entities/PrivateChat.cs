using EnsureThat;

namespace Domain.Entities;

public sealed class PrivateChat : Chat
{
    public PrivateChat(User creator, User interlocutor, string idempotencyKey)
        : base(creator, [creator, interlocutor], idempotencyKey)
    {
        EnsureArg.IsNotNull(creator, nameof(creator));
        EnsureArg.IsNotNull(interlocutor, nameof(interlocutor));
    }

#nullable disable
    protected PrivateChat()
    {
    }
#nullable restore

    public static PrivateChat Create(User creator, User interlocutor)
    {
        EnsureArg.IsNotNull(creator, nameof(creator));
        EnsureArg.IsNotNull(interlocutor, nameof(interlocutor));

        // sort users ids
        var idempotencyKey = creator.Id < interlocutor.Id
            ? $"{creator.Id}-{interlocutor.Id}"
            : $"{interlocutor.Id}-{creator.Id}";
        return new PrivateChat(creator, interlocutor, idempotencyKey);
    }
}
