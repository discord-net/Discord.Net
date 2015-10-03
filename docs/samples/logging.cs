class Program
{
    private static DiscordBotClient _client;
    static void Main(string[] args)
    {
        var client = new DiscordClient(new DiscordClientConfig {
			//Warning: Debug mode should only be used for identifying problems. It _will_ slow your application down.
			LogLevel = LogMessageSeverity.Debug
		});		
		client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");
                
        client.Run(async () =>
        {
            await client.Connect("discordtest@email.com", "Password123");
            if (!client.Servers.Any())
                await client.AcceptInvite("aaabbbcccdddeee");
        });
    }
}
