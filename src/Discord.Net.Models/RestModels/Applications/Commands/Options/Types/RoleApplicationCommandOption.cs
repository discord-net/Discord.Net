using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 8)]
public sealed class RoleApplicationCommandOption :
    ApplicationCommandOption,
    IRoleApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    bool? IRoleApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();
}