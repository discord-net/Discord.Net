public class CommandHandler
{
    private readonly CommandService _commands;
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _services;

    public CommandHandler(CommandService commands, DiscordSocketClient client, IServiceProvider services)
    {
        _commands = commands;
        _client = client;
        _services = services;
    }

    public async Task SetupAsync()
    {
        _client.MessageReceived += CommandHandleAsync;

        // Add BooleanTypeReader to type read for the type "bool"
        _commands.AddTypeReader(typeof(bool), new BooleanTypeReader());

        // Then register the modules
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    public async Task CommandHandleAsync(SocketMessage msg)
    {
        // ...
    }
}