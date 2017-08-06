#pragma warning disable CS1591
using Discord.Serialization;
using System.Collections.Generic;

namespace Discord.API.Rpc
{
    internal class GetChannelsResponse
    {
        [ModelProperty("channels")]
        public IReadOnlyCollection<ChannelSummary> Channels { get; set; }
    }
}
