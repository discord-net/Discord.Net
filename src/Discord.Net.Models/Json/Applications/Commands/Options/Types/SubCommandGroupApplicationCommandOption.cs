using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 2)]
public class SubCommandGroupApplicationCommandOption :
    ApplicationCommandOption,
    ISubCommandGroupApplicationCommandOptionModel
{
    [JsonPropertyName("options")]
    public Optional<ApplicationCommandOption[]> Options { get; set; }

    IReadOnlyCollection<IApplicationCommandOptionModel>? ISubCommandGroupApplicationCommandOptionModel.Options 
        => ~Options;
}