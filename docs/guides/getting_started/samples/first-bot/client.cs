private DiscordSocketClient _client;

public async Task MainAsync()
{
	_client = new DiscordSocketClient();

	_client.Log += Log;

	// Remember to keep token private or to read it from an 
	// external source! In this case, we are reading the token 
	// from an environment variable. If you do not know how to set-up
	// environment variables, you may find more information on the 
	// Internet or by using other methods such as reading from 
	// a configuration.
	await _client.LoginAsync(TokenType.Bot, 
		Environment.GetEnvironmentVariable("DiscordToken"));
	await _client.StartAsync();

	// Block this task until the program is closed.
	await Task.Delay(-1);
}