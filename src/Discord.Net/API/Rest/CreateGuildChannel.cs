using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreateChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"guilds/{GuildId}/channels";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public ChannelType Type { get; set; }
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }

        public CreateChannelRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
