using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class DeleteMessagesParams
    {
        [JsonPropertyName("messages")]
        public ulong[] MessageIds { get; }

        public DeleteMessagesParams(ulong[] messageIds)
        {
            MessageIds = messageIds;
        }
    }
}
