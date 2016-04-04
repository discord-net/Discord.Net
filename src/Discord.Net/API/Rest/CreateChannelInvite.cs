using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreateChannelInviteRequest : IRestRequest<Invite>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"channels/{ChannelId}/invites";
        object IRestRequest.Payload => this;

        public ulong ChannelId { get; }

        [JsonProperty("max_age")]
        public int MaxAge { get; set; } = 86400; //24 Hours
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; } = 0;
        [JsonProperty("temporary")]
        public bool IsTemporary { get; set; } = false;
        [JsonProperty("xkcdpass")]
        public bool WithXkcdPass { get; set; } = false;

        public CreateChannelInviteRequest(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}
