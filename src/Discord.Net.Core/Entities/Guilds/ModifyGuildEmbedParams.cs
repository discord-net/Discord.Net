namespace Discord
{
    public class ModifyGuildEmbedParams
    {
        public Optional<bool> Enabled { get; set; }
        public Optional<ulong?> ChannelId { get; set; }
    }
}
