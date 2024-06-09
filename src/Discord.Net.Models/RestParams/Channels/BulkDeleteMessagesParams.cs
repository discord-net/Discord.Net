using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class BulkDeleteMessagesParams
{
    [JsonPropertyName("messages")]
    public required ulong[] Messages { get; set; }
}
