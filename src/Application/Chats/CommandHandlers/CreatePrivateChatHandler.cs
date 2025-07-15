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

internal sealed class CreatePrivateChatHandler : IRequestHandler<CreatePrivateChat, ChatModel>
{
    public CreatePrivateChatHandler(IUsersRepository usersRepository, IChatsRepository chatsRepository)
    {
        EnsureArg.IsNotNull(usersRepository, nameof(usersRepository));
        EnsureArg.IsNotNull(chatsRepository, nameof(chatsRepository));

        this.usersRepository = usersRepository;
        this.chatsRepository = chatsRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<CreatePrivateChatHandler>();
    private readonly IUsersRepository usersRepository;
    private readonly IChatsRepository chatsRepository;

    public async Task<ChatModel> Handle(CreatePrivateChat command, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(command, nameof(command));

        Logger.Information("Creating private chat between actor {ActorId} and interlocutor {InterlocutorId}", command.ActorId, command.InterlocutorId);

        var actor = await this.usersRepository.GetAsync(command.ActorId, cancellationToken);
        var interlocutor = await this.usersRepository.GetAsync(command.InterlocutorId, cancellationToken);

        var chat = PrivateChat.Create(actor, interlocutor);

        this.chatsRepository.Insert(chat);

        await this.chatsRepository.SaveChangesAsync(cancellationToken);

        Logger.Information("Successfully created private chat with ID {ChatId} between users {ActorId} and {InterlocutorId}", chat.Id, command.ActorId, command.InterlocutorId);

        return chat.ToModel();
    }
}
