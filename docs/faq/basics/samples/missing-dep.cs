public class MyModule : ModuleBase<SocketCommandContext>
{
    private readonly DatabaseService _dbService;
    public MyModule(DatabaseService dbService)
        => _dbService = dbService;
}
public class CommandHandler
{
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    public CommandHandler(DiscordSocketClient client)
    {
        _services = new ServiceCollection()
            .AddSingleton<CommandService>()
            .AddSingleton(client)
            // We are missing DatabaseService!
            .BuildServiceProvider();
    }
    public async Task RegisterCommandsAsync()
    {
        // ...
        // The method fails here because DatabaseService is a required
        // dependency and cannot be resolved by the dependency
        // injection service at runtime since the service is not
        // registered in this instance of _services.
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        // ...

        // The same approach applies to the interaction service.
        // Make sure to resolve these issues!
    }
}
