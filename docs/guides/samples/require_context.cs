public class InfoModule : ModuleBase
{
    // Constrain this command to Guilds
    [RequireContext(ContextType.Guild)]
    public async Task Whois(IGuildUser user) { }

    // Constrain this command to either Guilds or DMs
    [RequireContext(ContextType.Guild | ContextType.DM)]
    public async Task Info() { }
}