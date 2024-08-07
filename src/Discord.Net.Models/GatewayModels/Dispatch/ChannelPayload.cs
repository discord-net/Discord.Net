using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class ChannelPayload : IChannelPayloadData
{
    [JsonIgnore, JsonExtend]
    public required Channel Channel { get; set; }
}
