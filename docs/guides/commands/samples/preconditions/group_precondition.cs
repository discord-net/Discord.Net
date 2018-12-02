// The following example only requires the user to either have the
// Administrator permission in this guild or own the bot application.
[RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
[RequireOwner(Group = "Permission")]
public class AdminModule : ModuleBase<SocketCommandContext>
{
    [Command("ban")]
    public Task BanAsync(IUser user) => Context.Guild.AddBanAsync(user);
}