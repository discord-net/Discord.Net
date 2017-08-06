#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class Channel
    {
        //Shared
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("type")]
        public ChannelType Type { get; set; }

        //GuildChannel
        [ModelProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [ModelProperty("name")]
        public Optional<string> Name { get; set; }
        [ModelProperty("position")]
        public Optional<int> Position { get; set; }

        //IMessageChannel
        [ModelProperty("messages")]
        public Message[] Messages { get; set; }

        //VoiceChannel
        [ModelProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }
        [ModelProperty("user_limit")]
        public Optional<int> UserLimit { get; set; }
        [ModelProperty("voice_states")]
        public ExtendedVoiceState[] VoiceStates { get; set; }
    }
}
