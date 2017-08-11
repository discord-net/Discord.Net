#pragma warning disable CS1591
using Discord.Serialization;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    internal class MessageDeleteBulkEvent
    {
        [ModelProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [ModelProperty("ids")]
        public ulong[] Ids { get; set; }
    }
}
