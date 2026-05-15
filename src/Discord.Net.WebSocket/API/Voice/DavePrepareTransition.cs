using Newtonsoft.Json;

namespace Discord.API.Voice;

internal class DavePrepareTransition
{
    [JsonProperty("transition_id")]
    public ushort TransitionId { get; set; }

    [JsonProperty("protocol_version")]
    public ushort ProtocolVersion { get; set; }
}
