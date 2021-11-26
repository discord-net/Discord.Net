public async Task MainAsync()
{
	// ...
	_client.MessageReceived += MessageReceived;
	// ...
}

private async Task MessageReceived(SocketMessage message)
{
	if (message.Content == "!ping")
	{
		await message.Channel.SendMessageAsync("Pong!");
	}
}
