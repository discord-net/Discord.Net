using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class GetWidgetRequest : IRestRequest<GetWidgetResponse>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"{DiscordConfig.ClientAPIUrl}/servers/{GuildId}/widget.json";
        object IRestRequest.Payload => null;
        bool IRestRequest.IsPrivate => false;

        public ulong GuildId { get; }

        public GetWidgetRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }

    public sealed class GetWidgetResponse
    {
        public sealed class Channel
        {
            [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
            public ulong Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("position")]
            public int Position { get; set; }
        }
        public sealed class User : UserReference
        {
            [JsonProperty("avatar_url")]
            public string AvatarUrl { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("game")]
            public UserGame Game { get; set; }
        }
        public sealed class UserGame
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        [JsonProperty("id"), JsonConverter(typeof(LongStringConverter))]
        public ulong Id { get; set; }
        [JsonProperty("channels")]
        public Channel[] Channels { get; set; }
        [JsonProperty("members")]
        public MemberReference[] Members { get; set; }
        [JsonProperty("instant_invite")]
        public string InstantInviteUrl { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
