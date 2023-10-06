---
uid: Guides.ContextCommands.Reveiving
title: Receiving Context Commands
---

# Receiving Context Menu events

User commands and Message commands have their own unique event just like the other interaction types. For user commands the event is `UserCommandExecuted` and for message commands the event is `MessageCommandExecuted`.

```cs
// For message commands
client.MessageCommandExecuted += MessageCommandHandler;

// For user commands
client.UserCommandExecuted += UserCommandHandler;

...

public async Task MessageCommandHandler(SocketMessageCommand arg)
{
    Console.WriteLine("Message command received!");
}

public async Task UserCommandHandler(SocketUserCommand arg)
{
    Console.WriteLine("User command received!");
}
```

User commands contain a SocketUser object called `Member` in their data class, showing the user that was clicked to run the command. 
Message commands contain a SocketMessage object called `Message` in their data class, showing the message that was clicked to run the command.

Both return the user who ran the command, the guild (if any), channel, etc.
