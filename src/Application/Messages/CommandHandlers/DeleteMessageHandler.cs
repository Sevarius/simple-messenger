using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Mappings;
using Application.Messages.Commands;
using Application.Repositories;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

namespace Application.Messages.CommandHandlers;

internal sealed class DeleteMessageHandler : IRequestHandler<DeleteMessage, MessageAndChatModel>
{
    public DeleteMessageHandler(
        IChatsRepository chatsRepository,
        IMessagesRepository messagesRepository)
    {
        EnsureArg.IsNotNull(chatsRepository, nameof(chatsRepository));
        EnsureArg.IsNotNull(messagesRepository, nameof(messagesRepository));

        this.chatsRepository = chatsRepository;
        this.messagesRepository = messagesRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<DeleteMessageHandler>();
    private readonly IChatsRepository chatsRepository;
    private readonly IMessagesRepository messagesRepository;

    public async Task<MessageAndChatModel> Handle(DeleteMessage command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        Logger.Information("Deleting message {MessageId} in chat {ChatId} by user {ActorId}", command.MessageId, command.ChatId, command.ActorId);

        var chat = await this.chatsRepository.GetAsync(command.ChatId, cancellationToken);

        var message = await this.messagesRepository.GetAsync(command.MessageId, cancellationToken);

        if (message.UserId != command.ActorId)
        {
            Logger.Warning("User {ActorId} attempted to delete message {MessageId} without being the author", command.ActorId, command.MessageId);
            throw new ApplicationException("Only the message author can delete the message.");
        }

        if (message.ChatId != command.ChatId)
        {
            Logger.Warning("Message {MessageId} does not belong to chat {ChatId}", command.MessageId, command.ChatId);
            throw new ApplicationException("Message does not belong to the specified chat.");
        }

        message.Delete();

        await this.messagesRepository.SaveChangesAsync(cancellationToken);

        Logger.Information("Successfully deleted message {MessageId} in chat {ChatId} by user {ActorId}", command.MessageId, command.ChatId, command.ActorId);

        return new MessageAndChatModel
        {
            Message = message.ToModel(),
            Chat = chat.ToModel()
        };
    }
}
