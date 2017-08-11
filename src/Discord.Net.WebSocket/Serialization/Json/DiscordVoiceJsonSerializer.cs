using System;

namespace Discord.Serialization.Json
{
    public class DiscordVoiceJsonSerializer : JsonSerializer
    {
        private static readonly Lazy<DiscordVoiceJsonSerializer> _singleton = new Lazy<DiscordVoiceJsonSerializer>();
        public static DiscordVoiceJsonSerializer Global => _singleton.Value;

        public DiscordVoiceJsonSerializer()
            : this((JsonSerializer)null) { }
        public DiscordVoiceJsonSerializer(JsonSerializer parent)
            : base(parent ?? DiscordRestJsonSerializer.Global)
        {
        }

        private DiscordVoiceJsonSerializer(DiscordVoiceJsonSerializer parent)
            : base(parent) { }
        public DiscordVoiceJsonSerializer CreateScope() => new DiscordVoiceJsonSerializer(this);
    }
}
