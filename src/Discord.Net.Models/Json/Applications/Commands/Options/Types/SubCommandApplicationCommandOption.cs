using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 1)]
public sealed class SubCommandApplicationCommandOption :
    ApplicationCommandOption,
    ISubCommandApplicationCommandOptionModel
{
    [JsonPropertyName("options")]
    public Optional<ApplicationCommandOption[]> Options { get; set; }

    IReadOnlyCollection<IApplicationCommandOptionModel>? ISubCommandApplicationCommandOptionModel.Options => ~Options;
}