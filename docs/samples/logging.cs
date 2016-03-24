class Program
{
    static void Main(string[] args)
    {
        var client = new DiscordClient(x =>
        {
			LogLevel = LogSeverity.Info
		});

        client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

        client.ExecuteAndWait(async () =>
        {
            await client.Connect("discordtest@email.com", "Password123");
            if (!client.Servers.Any())
                await (client.GetInvite("aaabbbcccdddeee")).Accept();
        });
    }
}
