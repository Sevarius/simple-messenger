using System;
using System.Linq;
using Client.Models;
using EnsureThat;

namespace Client;

public static class ConsoleWriter
{
    public static void OpenChat(ChatModel chat)
    {
        EnsureArg.IsNotNull(chat, nameof(chat));

        Console.WriteLine($"Opening chat with ID: {chat.Id}");
        Console.WriteLine("Users in this chat:");
        foreach (var user in chat.Users)
        {
            Console.WriteLine($"- {user.UserName} (ID: {user.Id})");
        }
    }

    public static void WriteMessage(UserModel user, MessageModel message)
    {
        EnsureArg.IsNotNull(user, nameof(user));
        EnsureArg.IsNotNull(message, nameof(message));

        Console.WriteLine($"[{message.CreatedAt}] {user.UserName}: {message.Content}");
    }

    public static void ListChats(ChatModel[] chats)
    {
        EnsureArg.IsNotNull(chats, nameof(chats));

        if (chats.Length == 0)
        {
            Console.WriteLine("No chats available.");
            return;
        }

        Console.WriteLine("Available Chats:");
        foreach (var chat in chats)
        {
            Console.WriteLine($"(ID: {chat.Id}); {string.Join(", ", chat.Users.Select(user => user.UserName))}");
        }
    }

    public static void ClearScreen()
        => Console.Clear();

    public static void ClearLine()
    {
        Console.WriteLine(new string(' ', Console.WindowWidth - 1));
        Console.SetCursorPosition(0, Console.CursorTop - 1);
    }
}
