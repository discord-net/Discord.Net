[Module]
public class AdminModule
{
    [Command("ban")]
    [RequirePermission(GuildPermission.BanMembers)]
    public async Task Ban(IMessage msg) { }
}