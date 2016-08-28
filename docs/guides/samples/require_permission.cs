[Module]
public class AdminModule
{
    [Command("ban")]
    [RequirePermission(GuildPermission.BanMembers)]
    public async Task Ban(IUserMessage msg) { }
}