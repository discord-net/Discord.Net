using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreateDMChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"users/@me/channels";
        object IRestRequest.Payload => this;

        [JsonProperty("recipient_id")]
        public ulong RecipientId { get; set; }
    }
}
