#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateDMChannelParams
    {
        [JsonProperty("recipient_id")]
        internal ulong _recipientId { get; set; }
        public ulong RecipientId { set { _recipientId = value; } }
        public IUser Recipient { set { _recipientId = value.Id; } }
    }
}
