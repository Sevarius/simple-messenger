using System;
using System.Linq;
using Client.Models;
using EnsureThat;
using Serilog;

namespace Client;

public static class ConsoleWriter
{
    private static readonly ILogger Logger = Log.ForContext(typeof(ConsoleWriter));

    public static void OpenChat(ChatModel chat)
    {
        EnsureArg.IsNotNull(chat, nameof(chat));

        Logger.Information("Displaying chat {ChatId} with {UserCount} users", chat.Id, chat.Users.Length);

        Console.WriteLine($"Opening chat with ID: {chat.Id}");
        Console.WriteLine("Users in this chat:");
        foreach (var user in chat.Users)
        {
            Console.WriteLine($"- {user.UserName} (ID: {user.Id})");
        }

        Logger.Information("Chat {ChatId} displayed successfully", chat.Id);
    }

    public static void WriteMessage(UserModel user, MessageModel message)
    {
        EnsureArg.IsNotNull(user, nameof(user));
        EnsureArg.IsNotNull(message, nameof(message));

        Logger.Information("Displaying message {MessageId} from user {UserId} ({UserName})", 
            message.Id, user.Id, user.UserName);

        Console.WriteLine($"[{message.CreatedAt}] {user.UserName}: {message.Content}");
    }

    public static void ListChats(ChatModel[] chats)
    {
        EnsureArg.IsNotNull(chats, nameof(chats));

        Logger.Information("Displaying {ChatCount} chats", chats.Length);

        if (chats.Length == 0)
        {
            Console.WriteLine("No chats available.");
            Logger.Information("No chats to display");
            return;
        }

        Console.WriteLine("Available Chats:");
        foreach (var chat in chats)
        {
            Console.WriteLine($"(ID: {chat.Id}); {string.Join(", ", chat.Users.Select(user => user.UserName))}");
        }

        Logger.Information("Successfully displayed {ChatCount} chats", chats.Length);
    }

    public static void ClearScreen()
    {
        Logger.Information("Clearing console screen");
        Console.Clear();
    }

    public static void ClearLine()
    {
        Logger.Information("Clearing console line");
        Console.WriteLine(new string(' ', Console.WindowWidth - 1));
        Console.SetCursorPosition(0, Console.CursorTop - 1);
    }
}
