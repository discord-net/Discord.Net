namespace Discord.Rest
{
    public struct ChannelInfo
    {
        internal ChannelInfo(string name, string topic, int? bitrate, int? limit)
        {
            Name = name;
            Topic = topic;
            Bitrate = bitrate;
            UserLimit = limit;
        }

        public string Name { get; }
        public string Topic { get; }
        public int? Bitrate { get; }
        public int? UserLimit { get; }
    }
}
