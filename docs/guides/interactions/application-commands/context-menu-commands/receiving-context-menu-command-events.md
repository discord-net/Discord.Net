# Receiving Context Menu events

User commands and Message commands have their own unique objects returned. Different from Slash commands. To get the appropriate object returned, you can use a similar method to the slash commands.

```cs
client.InteractionCreated += InteractionCreatedHandler;

...

public async Task InteractionCreatedHandler(SocketInteraction arg)
{
	if ( arg.Type == InteractionType.ApplicationCommand)
		Task.Run(() => ApplicationCommandHandler(arg));
}

public async Task ApplicationCommandHandler(SocketInteraction arg)
{
	switch (arg)
	{
		case SocketSlashCommand slashCommand:
			Console.Writeline("Slash command received!");
			break;
		case SocketUserCommand userCommand:
			Console.Writeline("User command received!")
			// userCommand.User = User who ran command.
			// userCommand.Data.Member = User who was clicked.
			break;
		case SocketMessageCommand messageCommand:
			Console.Writeline("Message command received!")
			// messageCommand.User = User who ran command.
			// messageCommand.Data.Message = Message that was clicked.
			break;
	}
}
```

User commands return a SocketUser object, showing the user that was clicked to run the command. 
Message commands return a SocketMessage object, showing the message that was clicked to run the command.

Both return the user who ran the command, the guild (if any), channel, etc.