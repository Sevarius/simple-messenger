using System;
using System.Linq;
using Client.Web.Models;
using EnsureThat;

namespace Client;

public static class ConsoleWriter
{
    public static void OpenChat(ChatResponse chat)
    {
        EnsureArg.IsNotNull(chat, nameof(chat));

        Console.WriteLine($"Opening chat with ID: {chat.Id}");
        Console.WriteLine($"Chat Type: {chat.ChatType}");
        Console.WriteLine("Users in this chat:");
        foreach (var user in chat.Users)
        {
            Console.WriteLine($"- {user.UserName} (ID: {user.Id})");
        }
    }

    public static void ListMessages(MessageResponse[] messages)
    {
        EnsureArg.IsNotNull(messages, nameof(messages));

        foreach (var message in messages)
        {
            WriteMessage(message);
        }
    }

    public static void WriteMessage(MessageResponse message)
    {
        EnsureArg.IsNotNull(message, nameof(message));

        Console.WriteLine($"[{message.CreatedAt}] {message.Content}");
    }

    public static void ListChats(ChatResponse[] chats)
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
            Console.WriteLine($"{chat.ChatType}: {string.Join(", ", chat.Users.Select(user => user.UserName))}; (ID: {chat.Id})");
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
