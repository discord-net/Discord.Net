namespace Discord.Models;

public interface IClientProvider
{
    IDiscordClient Client { get; }
}