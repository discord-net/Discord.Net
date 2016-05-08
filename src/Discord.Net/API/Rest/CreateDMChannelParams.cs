using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class CreateDMChannelParams
    {
        [JsonProperty("recipient_id")]
        public ulong RecipientId { get; set; }
    }
}
