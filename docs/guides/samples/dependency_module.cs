using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class ModuleA : ModuleBase
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

public class ModuleB
{

    // Public settable properties will be injected
    public AnnounceService { get; set; }

    // Public properties without setters will not
    public CommandService Commands { get; }

    // Public properties annotated with [DontInject] will not
    [DontInject]
    public NotificationService { get; set; }

    public ModuleB(CommandService commands)
    {
        Commands = commands;
    }

}
