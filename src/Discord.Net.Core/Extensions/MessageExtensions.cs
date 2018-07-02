namespace Discord
{
    public static class MessageExtensions
    {
        public static string GetJumpUrl(this IMessage msg)
        {
            var channel = msg.Channel;
            return $"https://discordapp.com/channels/{(channel is IDMChannel ? "@me" : $"{(channel as IGuildChannel).GuildId}")}/{channel.Id}/{msg.Id}";
        }
    }
}
