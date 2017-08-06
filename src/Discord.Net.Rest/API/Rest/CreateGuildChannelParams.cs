#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class CreateGuildChannelParams
    {
        [ModelProperty("name")]
        public string Name { get; }
        [ModelProperty("type")]
        public ChannelType Type { get; }

        [ModelProperty("bitrate")]
        public Optional<int> Bitrate { get; set; }

        public CreateGuildChannelParams(string name, ChannelType type)
        {
            Name = name;
            Type = type;
        }
    }
}
