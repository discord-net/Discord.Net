using System;

namespace Discord.Serialization.Json
{
    public class DiscordRpcJsonSerializer : JsonSerializer
    {
        private static readonly Lazy<DiscordRpcJsonSerializer> _singleton = new Lazy<DiscordRpcJsonSerializer>();
        public static DiscordRpcJsonSerializer Global => _singleton.Value;

        public DiscordRpcJsonSerializer()
            : this((JsonSerializer)null) { }
        public DiscordRpcJsonSerializer(JsonSerializer parent)
            : base(parent ?? DiscordRestJsonSerializer.Global)
        {
        }

        private DiscordRpcJsonSerializer(DiscordRpcJsonSerializer parent)
            : base(parent) { }
        public DiscordRpcJsonSerializer CreateScope() => new DiscordRpcJsonSerializer(this);
    }
}
