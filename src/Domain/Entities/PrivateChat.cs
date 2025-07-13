using EnsureThat;

namespace Domain.Entities;

public sealed class PrivateChat : Chat
{
    public PrivateChat(User firstInterlocutor, User secondInterlocutor)
        : base([firstInterlocutor, secondInterlocutor])
    {
        EnsureArg.IsNotNull(firstInterlocutor, nameof(firstInterlocutor));
        EnsureArg.IsNotNull(secondInterlocutor, nameof(secondInterlocutor));
    }

#nullable disable
    protected PrivateChat()
    {
    }
#nullable restore
}
