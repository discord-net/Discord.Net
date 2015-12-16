using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UpdateGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/guilds/{GuildId}";
        object IRestRequest.Payload => this;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("icon")]
        public string IconBase64 { get; set; }
        [JsonProperty("afk_channel_id"), JsonConverter(typeof(NullableLongStringConverter))]
        public ulong? AFKChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int AFKTimeout { get; set; }

        public UpdateGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
