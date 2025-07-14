using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Commands;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using MediatR;

namespace Application.Chats.CommandHandlers;

internal sealed class CreatePrivateChatHandler : IRequestHandler<CreatePrivateChat, Guid>
{
    public CreatePrivateChatHandler(IUsersRepository usersRepository, IChatsRepository chatsRepository)
    {
        EnsureArg.IsNotNull(usersRepository, nameof(usersRepository));
        EnsureArg.IsNotNull(chatsRepository, nameof(chatsRepository));

        this.usersRepository = usersRepository;
        this.chatsRepository = chatsRepository;
    }

    private readonly IUsersRepository usersRepository;
    private readonly IChatsRepository chatsRepository;

    public async Task<Guid> Handle(CreatePrivateChat command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var actor = await this.usersRepository.GetAsync(command.ActorId, cancellationToken);
        var interlocutor = await this.usersRepository.GetAsync(command.InterlocutorId, cancellationToken);

        var chat = new PrivateChat(actor, interlocutor);

        this.chatsRepository.Insert(chat);

        await this.chatsRepository.SaveChangesAsync(cancellationToken);

        return chat.Id;
    }
}
