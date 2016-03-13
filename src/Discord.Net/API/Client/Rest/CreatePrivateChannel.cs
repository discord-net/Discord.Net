using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CreatePrivateChannelRequest : IRestRequest<Channel>
    {
        string IRestRequest.Method => "POST";
        string IRestRequest.Endpoint => $"users/@me/channels";
        object IRestRequest.Payload => this;

        [JsonProperty("recipient_id"), JsonConverter(typeof(LongStringConverter))]
        public ulong RecipientId { get; set; }
    }
}
