// Program.cs
using Discord.WebSocket;
// ...
public async Task MainAsync()
{
	var client = new DiscordSocketClient();

	client.Log += Log;

	string token = "abcdefg..."; // Remember to keep this private!
	await client.LoginAsync(TokenType.Bot, token);
	await client.StartAsync();

	// Block this task until the program is closed.
	await Task.Delay(-1);
}