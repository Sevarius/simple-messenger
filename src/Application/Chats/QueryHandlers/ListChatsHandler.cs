using System.Threading;
using System.Threading.Tasks;
using Application.Chats.Queries;
using Application.Repositories;
using EnsureThat;
using MediatR;
using Models;
using Serilog;

namespace Application.Chats.QueryHandlers;

internal sealed class ListChatsHandler : IRequestHandler<ListChats, ChatModel[]>
{
    public ListChatsHandler(IChatsReadOnlyRepository chatsReadOnlyRepository)
    {
        EnsureArg.IsNotNull(chatsReadOnlyRepository, nameof(chatsReadOnlyRepository));

        this.chatsReadOnlyRepository = chatsReadOnlyRepository;
    }

    private static readonly ILogger Logger = Log.ForContext<ListChatsHandler>();
    private readonly IChatsReadOnlyRepository chatsReadOnlyRepository;

    public async Task<ChatModel[]> Handle(ListChats query, CancellationToken cancellationToken)
    {
        EnsureArg.IsNotNull(query, nameof(query));

        Logger.Information("Listing chats for user {UserId}", query.UserId);

        var chats = await this.chatsReadOnlyRepository.ListAsync(query.UserId, cancellationToken);

        Logger.Information("Successfully retrieved {ChatCount} chats for user {UserId}", chats.Length, query.UserId);

        return chats;
    }
}
