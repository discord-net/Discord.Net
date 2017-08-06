#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class CreateDMChannelParams
    {
        [ModelProperty("recipient_id")]
        public ulong RecipientId { get; }

        public CreateDMChannelParams(ulong recipientId)
        {
            RecipientId = recipientId;
        }
    }
}
