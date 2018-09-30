public class Initialize
{
	private readonly CommandService _commands;
	private readonly DiscordSocketClient _client;

	public Initialize(CommandService commands = null, DiscordSocketClient client = null)
	{
		_commands = commands ?? new CommandService();
		_client = client ?? new DiscordSocketClient();
	}

	public IServiceProvider BuildServiceProvider() => new ServiceCollection()
		.AddSingleton(_client)
		.AddSingleton(_commands)
		// You can pass in an instance of the desired type
		.AddSingleton(new NotificationService())
		// ...or by using the generic method.
		//
		// The benefit of using the generic method is that 
		// ASP.NET DI will attempt to inject the required
		// dependencies that are specified under the constructor 
		// for us.
		.AddSingleton<DatabaseService>()
		.AddSingleton<CommandHandler>()
		.BuildServiceProvider();
}
public class CommandHandler
{
	private readonly DiscordSocketClient _client;
	private readonly CommandService _commands;
	private readonly IServiceProvider _services;

	public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client)
	{
		_commands = commands;
		_services = services;
		_client = client;
	}

	public async Task InitializeAsync()
	{
		// Pass the service provider to the second parameter of
		// AddModulesAsync to inject dependencies to all modules 
		// that may require them.
		await _commands.AddModulesAsync(
			assembly: Assembly.GetEntryAssembly(), 
			services: _services);
		_client.MessageReceived += HandleCommandAsync;
	}

	public async Task HandleCommandAsync(SocketMessage msg)
	{
		// ...
		// Pass the service provider to the ExecuteAsync method for
		// precondition checks.
		await _commands.ExecuteAsync(
			context: context, 
			argPos: argPos, 
			services: _services);
		// ...
	}
}
