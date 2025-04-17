using Newtonsoft.Json;
using System;

namespace Discord.API;

internal class MessageCallData
{
    [JsonProperty("ended_timestamp")]
    public Optional<DateTimeOffset> EndedTimestamp { get; set; }

    [JsonProperty("participants")]
    public ulong[] Participants { get; set; }
}
