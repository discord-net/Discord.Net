namespace Discord
{
    public static class NsfwUtils
    {
        public static bool IsNsfw(IChannel channel) =>
            IsNsfw(channel.Name);
        public static bool IsNsfw(string channelName) =>
            channelName.StartsWith("nsfw");
    }
}
