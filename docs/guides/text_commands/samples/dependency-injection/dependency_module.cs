// After setting up dependency injection, modules will need to request
// the dependencies to let the library know to pass 
// them along during execution.

// Dependency can be injected in two ways with Discord.Net.
// You may inject any required dependencies via...
// the module constructor
// -or-
// public settable properties

// Injection via constructor
public class DatabaseModule : ModuleBase<SocketCommandContext>
{
    private readonly DatabaseService _database;
    public DatabaseModule(DatabaseService database)
    {
        _database = database;
    }

    [Command("read")]
    public async Task ReadFromDbAsync()
    {
        await ReplyAsync(_database.GetData());
    }
}

// Injection via public settable properties
public class DatabaseModule : ModuleBase<SocketCommandContext>
{
    public DatabaseService DbService { get; set; }

    [Command("read")]
    public async Task ReadFromDbAsync()
    {
        await ReplyAsync(DbService.GetData());
    }
}
