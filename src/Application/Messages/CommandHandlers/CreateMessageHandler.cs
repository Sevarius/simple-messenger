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

    private static readonly ILogger Logger = Log.ForContext<CreateMessageHandler>();
    private readonly IUsersRepository usersRepository;
    private readonly IChatsRepository chatsRepository;
    private readonly IMessagesRepository messagesRepository;

    public async Task<MessageModel> Handle(CreateMessage command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        Logger.Information("Creating message for chat {ChatId} by user {ActorId}", command.ChatId, command.ActorId);

        var actor = await this.usersRepository.GetAsync(command.ActorId, cancellationToken);

        var chat = await this.chatsRepository.GetAsync(command.ChatId, cancellationToken);

        if (!chat.Users.Contains(actor))
        {
            Logger.Warning("User {ActorId} attempted to send message to chat {ChatId} without being a member", command.ActorId, command.ChatId);
            throw new ApplicationException("User is not a member of the chat.");
        }

        var message = new Message(command.ChatId, command.ActorId, command.Content);

        this.messagesRepository.Insert(message);

        await this.messagesRepository.SaveChangesAsync(cancellationToken);

        Logger.Information("Successfully created message with ID {MessageId} for chat {ChatId} by user {ActorId}", message.Id, command.ChatId, command.ActorId);

        return message.ToModel();
    }
}
