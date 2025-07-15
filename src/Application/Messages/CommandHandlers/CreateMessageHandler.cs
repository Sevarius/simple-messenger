using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Mappings;
using Application.Messages.Commands;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using MediatR;
using Models;

namespace Application.Messages.CommandHandlers;

internal sealed class CreateMessageHandler : IRequestHandler<CreateMessage, MessageModel>
{
    public CreateMessageHandler(
        IUsersRepository usersRepository,
        IChatsRepository chatsRepository,
        IMessagesRepository messagesRepository)
    {
        EnsureArg.IsNotNull(usersRepository, nameof(usersRepository));
        EnsureArg.IsNotNull(chatsRepository, nameof(chatsRepository));
        EnsureArg.IsNotNull(messagesRepository, nameof(messagesRepository));

        this.usersRepository = usersRepository;
        this.chatsRepository = chatsRepository;
        this.messagesRepository = messagesRepository;
    }

    private readonly IUsersRepository usersRepository;
    private readonly IChatsRepository chatsRepository;
    private readonly IMessagesRepository messagesRepository;

    public async Task<MessageModel> Handle(CreateMessage command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var actor = await this.usersRepository.GetAsync(command.ActorId, cancellationToken);

        var chat = await this.chatsRepository.GetAsync(command.ChatId, cancellationToken);

        if (!chat.Users.Contains(actor))
        {
            throw new ApplicationException("User is not a member of the chat.");
        }

        var message = new Message(command.ActorId, command.ChatId, command.Content);

        this.messagesRepository.Insert(message);

        await this.messagesRepository.SaveChangesAsync(cancellationToken);

        return message.ToModel();
    }
}
