#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class Channel
    {
        //Shared
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("type")]
        public ChannelType Type { get; set; }

        //GuildChannel
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("position")]
        public Optional<int> Position { get; set; }

        //IMessageChannel
        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        //VoiceChannel
        [JsonProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [JsonProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }
        [JsonProperty("voice_states")]
        public ExtendedVoiceState[] VoiceStates { get; set; }
    }
}
