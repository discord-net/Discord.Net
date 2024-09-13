using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 6)]
public sealed class UserApplicationCommandOption :
    ApplicationCommandOption,
    IUserApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    bool? IUserApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();
}