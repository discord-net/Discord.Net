using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 7)]
public sealed class ChannelApplicationCommandOption :
    ApplicationCommandOption,
    IChannelApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }
    
    [JsonPropertyName("channel_types")]
    public Optional<int[]> ChannelTypes { get; set; }

    int[]? IChannelApplicationCommandOptionModel.ChannelTypes => ~ChannelTypes;

    bool? IChannelApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();
}