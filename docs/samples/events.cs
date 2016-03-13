class Program
{
	private static DiscordClient _client;
	static void Main(string[] args)
	{
		_client = new DiscordClient();

		// Handle Events using Lambdas
		_client.MessageReceived += (s, e) =>
		{
			if (!e.Message.IsAuthor)
				await e.Channel.SendMessage("foo");
		}

		// Handle Events using Event Handlers
		EventHandler<MessageEventArgs> handler = new EventHandler<MessageEventArgs>(HandleMessageCreated);
		client.MessageReceived += handler;
	}


	// NOTE: When using this method, 'client' must be accessible from outside the Main function.
	static void HandleMessageCreated(object sender, EventArgs e)
	{
		if (!e.Message.IsAuthor)
			await e.Channel.SendMessage("bar");
	}
}
