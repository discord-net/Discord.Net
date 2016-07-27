using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateDMChannelParams
    {
        [JsonProperty("recipient_id")]
        internal ulong _recipientId;
        public ulong RecipientId { set { _recipientId = value; } }
        public IUser Recipient { set { _recipientId = value.Id; } }
    }
}
