public class DatabaseModule : ModuleBase<SocketCommandContext>
{
    private readonly DatabaseService _database;

    // Dependencies can be injected via the constructor
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

public class MixModule : ModuleBase<SocketCommandContext>
{
    // Public settable properties will be injected
    public AnnounceService AnnounceService { get; set; }

    // Public properties without setters will not be injected
    public ImageService ImageService { get; }

    // Public properties annotated with [DontInject] will not
    // be injected
    [DontInject]
    public NotificationService NotificationService { get; set; }
}