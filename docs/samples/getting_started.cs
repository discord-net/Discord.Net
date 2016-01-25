class Program
{
	static void Main(string[] args)
	{
		var client = new DiscordClient();

		//Display all log messages in the console
		client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

		//Echo back any message received, provided it didn't come from the bot itself
		client.MessageReceived += async (s, e) =>
		{
			if (!e.Message.IsAuthor)
				await e.Channel.SendMessage(e.Message.Text);
		};

		//Convert our sync method to an async one and block the Main function until the bot disconnects
		client.ExecuteAndWait(async () =>
		{
			//Connect to the Discord server using our email and password
			await client.Connect("discordtest@email.com", "Password123");

			//If we are not a member of any server, use our invite code (made beforehand in the official Discord Client)
			if (!client.Servers.Any())
				await client.AcceptInvite(client.GetInvite("aaabbbcccdddeee"));
		});
	}
}
