#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class DeleteMessagesParams
    {
        [ModelProperty("messages")]
        public ulong[] MessageIds { get; }

        public DeleteMessagesParams(ulong[] messageIds)
        {
            MessageIds = messageIds;
        }
    }
}
