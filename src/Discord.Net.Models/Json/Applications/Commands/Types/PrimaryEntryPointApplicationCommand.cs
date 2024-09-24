using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 4)]
public sealed class PrimaryEntryPointApplicationCommand :
    ApplicationCommand,
    IPrimaryEntryPointApplicationCommandModel
{
    [JsonPropertyName("handler")]
    public Optional<int> Handler { get; set; }

    int? IPrimaryEntryPointApplicationCommandModel.Handler => Handler.ToNullable();
}