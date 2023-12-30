using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class BulkDeleteMessagesParams
{
    [JsonPropertyName("messages")]
    public required ulong[] Messages { get; set; }
}
