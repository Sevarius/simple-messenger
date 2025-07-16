using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Commands;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using MediatR;
using Serilog;

namespace Application.Chats.CommandHandlers;

internal sealed class RemoveUserFromGroupChatHandler : IRequestHandler<RemoveUserFromGroupChat>
{
    public RemoveUserFromGroupChatHandler(IUsersRepository usersRepository, IChatsRepository chatsRepository)
    {
        EnsureArg.IsNotNull(usersRepository, nameof(usersRepository));
        EnsureArg.IsNotNull(chatsRepository, nameof(chatsRepository));

        this.usersRepository = usersRepository;
        this.chatsRepository = chatsRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<RemoveUserFromGroupChatHandler>();
    private readonly IUsersRepository usersRepository;
    private readonly IChatsRepository chatsRepository;

    public async Task Handle(RemoveUserFromGroupChat command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        Logger.Information("Removing user {UserId} from group chat {ChatId} by actor {ActorId}", command.UserId, command.ChatId, command.ActorId);

        var chat = await this.chatsRepository.GetAsync(command.ChatId, cancellationToken).ConfigureAwait(false);
        var userToRemove = await this.usersRepository.GetAsync(command.UserId, cancellationToken).ConfigureAwait(false);

        if (chat is not GroupChat groupChat)
        {
            throw new InvalidOperationException($"Chat {command.ChatId} is not a group chat");
        }

        groupChat.RemoveUser(userToRemove);

        await this.chatsRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        Logger.Information("Successfully removed user {UserId} from group chat {ChatId}", command.UserId, command.ChatId);
    }
}
