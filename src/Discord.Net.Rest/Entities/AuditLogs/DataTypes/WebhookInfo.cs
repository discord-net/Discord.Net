namespace Discord.Rest
{
    public struct WebhookInfo
    {
        internal WebhookInfo(string name, ulong? channelId, string avatar)
        {
            Name = name;
            ChannelId = channelId;
            Avatar = avatar;
        }

        public string Name { get; }
        public ulong? ChannelId { get; }
        public string Avatar { get; }
    }
}
