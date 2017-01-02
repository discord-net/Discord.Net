#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class DeleteMessagesParams
    {
        [JsonProperty("messages")]
        public ulong[] MessageIds { get; }

        public DeleteMessagesParams(ulong[] messageIds)
        {
            MessageIds = messageIds;
        }
    }
}
