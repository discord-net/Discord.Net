using Newtonsoft.Json;

namespace Discord.API.Voice;

internal class DavePrepareEpoch
{
    [JsonProperty("protocol_version")]
    public ushort ProtocolVersion { get; set; }

    [JsonProperty("epoch")]
    public ulong Epoch { get; set; }
}
