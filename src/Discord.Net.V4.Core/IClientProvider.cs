namespace Discord;

public interface IClientProvider
{
    IDiscordClient Client { get; }
}
