public class AdminModule : ModuleBase
{
    [Command("ban")]
    [RequirePermission(GuildPermission.BanMembers)]
    public async Task Ban(IGuildUser target) { }
}