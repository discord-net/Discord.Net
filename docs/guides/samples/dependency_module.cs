using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class ModuleA : ModuleBase
{
    private readonly DatabaseService _database;

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
    private CommandService _commands;
    private NotificationService _notification;
    
    public ModuleB(CommandService commands, IDependencyMap map)
    {
        _commands = commands;
        _notification = map.Get<NotificationService>();
    }
}