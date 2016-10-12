#pragma warning disable CS1591
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rpc
{
    public class GetChannelsResponse
    {
        [JsonProperty("channels")]
        public IReadOnlyCollection<ChannelSummary> Channels { get; set; }
    }
}
