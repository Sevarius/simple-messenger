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
using Serilog;

namespace Application.Messages.CommandHandlers;

internal sealed class CreateMessageHandler : IRequestHandler<CreateMessage, MessageAndChatModel>
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

    private static readonly ILogger Logger = Log.ForContext<CreateMessageHandler>();
    private readonly IUsersRepository usersRepository;
    private readonly IChatsRepository chatsRepository;
    private readonly IMessagesRepository messagesRepository;

    public async Task<MessageAndChatModel> Handle(CreateMessage command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        var actor = await this.usersRepository.GetAsync(command.ActorId, cancellationToken);

        var chat = await this.chatsRepository.GetAsync(command.ChatId, cancellationToken);

        if (!chat.Users.Contains(actor))
        {
            Logger.Warning("User {ActorId} attempted to send message to chat {ChatId} without being a member", command.ActorId, command.ChatId);
            throw new ApplicationException("User is not a member of the chat.");
        }

        var message = new Message(command.ChatId, command.ActorId, command.Content);

        this.messagesRepository.Insert(message);

        chat.SetLastMessageTimestamp(message.CreatedAt);

        await this.messagesRepository.SaveChangesAsync(cancellationToken);

        return new MessageAndChatModel
        {
            Message = message.ToModel(),
            Chat = chat.ToModel()
        };
    }
}
