// Program.cs
using Discord.WebSocket;
// ...
private DiscordSocketClient _client;
public async Task MainAsync()
{
	_client = new DiscordSocketClient();

	_client.Log += Log;

	string token = "abcdefg..."; // Remember to keep this private!
	await _client.LoginAsync(TokenType.Bot, token);
	await _client.StartAsync();

	// Block this task until the program is closed.
	await Task.Delay(-1);
}
