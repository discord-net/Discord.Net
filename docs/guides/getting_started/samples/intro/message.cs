public async Task MainAsync()
{
	// client.Log ...
	client.MessageReceived += MessageReceived;
	// ...
}

private async Task MessageReceived(SocketMessage message)
{
	if (message.Content == "!ping")
	{
		await message.Channel.SendMessageAsync("Pong!");
	}
}