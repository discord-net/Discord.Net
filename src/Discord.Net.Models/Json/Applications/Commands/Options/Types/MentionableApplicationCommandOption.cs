using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 9)]
public sealed class MentionableApplicationCommandOption :
    ApplicationCommandOption,
    IMentionableApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    bool? IMentionableApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();
}