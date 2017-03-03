namespace Discord
{
    public class GuildIntegrationProperties
    {
        public Optional<int> ExpireBehavior { get; set; }
        public Optional<int> ExpireGracePeriod { get; set; }
        public Optional<bool> EnableEmoticons { get; set; }
    }
}
