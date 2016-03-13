using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; set; }

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
        [JsonProperty("splash")]
        public object Splash { get; set; }

        public UpdateGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
