namespace Discord
{
    public sealed class DiscordClientBuilder
    {
        public IDiscordClient FromConfig(DiscordClientConfig config)
        {
            return new DiscordClient(config);
        }
    }
}
