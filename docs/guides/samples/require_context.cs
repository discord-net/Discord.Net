[Module]
public class InfoModule
{
    // Constrain this command to Guilds
    [RequireContext(ContextType.Guild)]
    public async Task Whois(IUserMessage msg, IGuildUser user) { }

    // Constrain this command to either Guilds or DMs
    [RequireContext(ContextType.Guild | ContextType.DM)]
    public async Task Info(IUserMessage msg) { }
}