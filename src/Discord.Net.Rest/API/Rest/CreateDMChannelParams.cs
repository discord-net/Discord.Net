#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateDMChannelParams
    {
        public CreateDMChannelParams(ulong recipientId)
        {
            RecipientId = recipientId;
        }

        [JsonProperty("recipient_id")] public ulong RecipientId { get; }
    }
}
