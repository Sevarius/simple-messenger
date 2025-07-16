using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Commands;
using Application.Mappings;
using Application.Repositories;
using Domain.Entities;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

namespace Application.Chats.CommandHandlers;

internal sealed class CreateGroupChatHandler : IRequestHandler<CreateGroupChat, ChatModel>
{
    public CreateGroupChatHandler(IUsersRepository usersRepository, IChatsRepository chatsRepository)
    {
        EnsureArg.IsNotNull(usersRepository, nameof(usersRepository));
        EnsureArg.IsNotNull(chatsRepository, nameof(chatsRepository));

        this.usersRepository = usersRepository;
        this.chatsRepository = chatsRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<CreateGroupChatHandler>();
    private readonly IUsersRepository usersRepository;
    private readonly IChatsRepository chatsRepository;

    public async Task<ChatModel> Handle(CreateGroupChat command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        Logger.Information("Creating group chat with name {Name} by creator {CreatorId}", command.Name, command.CreatorId);

        var creator = await this.usersRepository.GetAsync(command.CreatorId, cancellationToken).ConfigureAwait(false);

        var chat = new GroupChat(creator, command.Name);

        this.chatsRepository.Insert(chat);

        await this.chatsRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        Logger.Information("Successfully created group chat with ID {ChatId} and name {Name} by creator {CreatorId}", chat.Id, command.Name, command.CreatorId);

        return chat.ToModel();
    }
} 