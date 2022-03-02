using Discord;

public class SampleModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ApplicationDbContext _db;

    public SampleModule(ApplicationDbContext db)
    {
        _db = db;
    }

    [SlashCommand("sample", "sample")]
    public async Task Sample()
    {
        // Do stuff with your injected DbContext
        var user = _db.Users.FirstOrDefault(x => x.Id == Context.User.Id);

        ...
  }
}
