using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreateInviteRequest : IRestRequest<Invite>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/invites";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; set; }

        [JsonProperty("max_age")]
        public int MaxAge { get; set; } = 1800;
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; } = 0;
        [JsonProperty("temporary")]
        public bool IsTemporary { get; set; } = false;
        [JsonProperty("xkcdpass")]
        public bool WithXkcdPass { get; set; } = false;
        /*[JsonProperty("validate")]
        public bool Validate { get; set; }*/

        public CreateInviteRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}
