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

internal sealed class AddUserToGroupChatHandler : IRequestHandler<AddUserToGroupChat>
{
    public AddUserToGroupChatHandler(IUsersRepository usersRepository, IChatsRepository chatsRepository)
    {
        EnsureArg.IsNotNull(usersRepository, nameof(usersRepository));
        EnsureArg.IsNotNull(chatsRepository, nameof(chatsRepository));

        this.usersRepository = usersRepository;
        this.chatsRepository = chatsRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<AddUserToGroupChatHandler>();
    private readonly IUsersRepository usersRepository;
    private readonly IChatsRepository chatsRepository;

    public async Task Handle(AddUserToGroupChat command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        Logger.Information("Adding user {UserId} to group chat {ChatId} by actor {ActorId}", command.UserId, command.ChatId, command.ActorId);

        var chat = await this.chatsRepository.GetAsync(command.ChatId, cancellationToken).ConfigureAwait(false);
        var userToAdd = await this.usersRepository.GetAsync(command.UserId, cancellationToken).ConfigureAwait(false);

        if (chat is not GroupChat groupChat)
        {
            throw new InvalidOperationException($"Chat {command.ChatId} is not a group chat");
        }

        groupChat.AddUser(userToAdd);

        await this.chatsRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        Logger.Information("Successfully added user {UserId} to group chat {ChatId}", command.UserId, command.ChatId);
    }
} 