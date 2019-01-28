namespace Discord
{
    public class DiscordClientConfig
    {
        public string Token { get; set; }

        public int Shard { get; set; } = 0;
        public int TotalShards { get; set; } = 1;
    }
}
