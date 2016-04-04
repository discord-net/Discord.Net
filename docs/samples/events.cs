class Program
{
	private static DiscordBotClient _client;
	static void Main(string[] args)
	{
		var client = new DiscordClient();

		// Handle Events using Lambdas
		client.MessageCreated += (s, e) =>
		{
			if (!e.Message.IsAuthor)
				await client.SendMessage(e.Message.ChannelId, "foo");
		}

		// Handle Events using Event Handlers
		EventHandler<MessageEventArgs> handler = new EventHandler<MessageEventArgs>(HandleMessageCreated);
		client.MessageCreated += handler;
	}


	// NOTE: When using this method, 'client' must be accessible from outside the Main function.
	static void HandleMessageCreated(object sender, EventArgs e)
	{
		if (!e.Message.IsAuthor)
				await client.SendMessage(e.Message.ChannelId, "foo");
	}
}