using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AutoModerationAction : IAutoModerationActionModel
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [
        JsonPropertyName("metadata"),
        JsonIgnore,
        DiscriminatedUnion(nameof(Type)),
        DiscriminatedUnionEntry<BlockMessageMetadata>(1),
        DiscriminatedUnionEntry<SendAlertMessageMetadata>(2),
        DiscriminatedUnionEntry<TimeoutMetadata>(3)
    ]
    public Optional<ActionMetadata> Metadata { get; set; }

    IAutoModerationActionMetadataModel? IAutoModerationActionModel.Metadata => ~Metadata;
}
