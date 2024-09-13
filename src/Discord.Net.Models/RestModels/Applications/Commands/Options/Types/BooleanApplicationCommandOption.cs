using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 5)]
public sealed class BooleanApplicationCommandOption :
    ApplicationCommandOption,
    IBooleanApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    bool? IBooleanApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();
}