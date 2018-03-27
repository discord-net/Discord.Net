using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class ModuleA : ModuleBase<SocketCommandContext>
{
    private readonly DatabaseService _database;

    // Dependencies can be injected via the constructor
    public ModuleA(DatabaseService database)
    {
        _database = database;
    }

    public async Task ReadFromDb()
    {
        var x = _database.getX();
        await ReplyAsync(x);
    }
}

public class ModuleB : ModuleBase<SocketCommandContext>
{
    // Public settable properties will be injected.
    public AnnounceService Announce { get; set; }

    // Public properties without setters will not be injected.
    public CommandService Commands { get; }

    // Public properties annotated with [DontInject] will not
    // be injected.
    [DontInject]
    public NotificationService NotificationService { get; set; }

    public ModuleB(CommandService commands)
    {
        Commands = commands;
    }

}
