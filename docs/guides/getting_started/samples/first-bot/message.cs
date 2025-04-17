public static async Task Main()
{
	// ...
	_client.MessageReceived += MessageReceived;
	// ...
}

private static async Task MessageReceived(SocketMessage message)
{
	if (message.Content == "!ping")
	{
		await message.Channel.SendMessageAsync("Pong!");
	}
}
