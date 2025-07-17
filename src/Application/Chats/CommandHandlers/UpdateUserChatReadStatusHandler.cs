using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Commands;
using Application.Mappings;
using Application.Repositories;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

namespace Application.Chats.CommandHandlers;

internal sealed class UpdateUserChatReadStatusHandler : IRequestHandler<UpdateUserChatReadStatus, ChatModel>
{
    public UpdateUserChatReadStatusHandler(IChatsRepository chatsRepository)
    {
        EnsureArg.IsNotNull(chatsRepository, nameof(chatsRepository));

        this.chatsRepository = chatsRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<UpdateUserChatReadStatusHandler>();
    private readonly IChatsRepository chatsRepository;

    public async Task<ChatModel> Handle(UpdateUserChatReadStatus command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        Logger.Information("Updating read status for user {UserId} in chat {ChatId} up to {LastReadMessageId}",
            command.UserId, command.ChatId, command.LastReadMessageTimestamp);

        var chat = await this.chatsRepository.GetAsync(command.ChatId, cancellationToken);

        var statusWasChanged = chat.UpdateUserChatReadStatus(command.UserId, command.LastReadMessageTimestamp);

        if (statusWasChanged)
        {
            Logger.Information("Updated read status for user {UserId} in chat {ChatId} to timestamp {Timestamp}",
                command.UserId, command.ChatId, command.LastReadMessageTimestamp);
        }

        return chat.ToModel();
    }
}
