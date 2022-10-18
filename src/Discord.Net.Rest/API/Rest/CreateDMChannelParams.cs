using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateDMChannelParams
    {
        [JsonPropertyName("recipient_id")]
        public ulong RecipientId { get; }

        public CreateDMChannelParams(ulong recipientId)
        {
            RecipientId = recipientId;
        }
    }
}
