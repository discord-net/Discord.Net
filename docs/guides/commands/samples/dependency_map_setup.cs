private IServiceProvider _services;
private CommandService _commands;

public async Task InstallAsync(DiscordSocketClient client)
{
	// Here, we will inject the ServiceProvider with 
	// all of the services our client will use.
	_services = new ServiceCollection()
		.AddSingleton(client)
		.AddSingleton(_commands)
		// You can pass in an instance of the desired type
		.AddSingleton(new NotificationService())
		// ...or by using the generic method.
		.AddSingleton<DatabaseService>()
		.BuildServiceProvider();
	// ...
	await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
}