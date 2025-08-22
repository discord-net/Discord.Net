namespace Discord.Models;

public interface IDiscordClient
{
    DiscordConfig Config { get; }
    
    IUsersLink Users { get; }
    IGuildsLink Guilds { get; }
}