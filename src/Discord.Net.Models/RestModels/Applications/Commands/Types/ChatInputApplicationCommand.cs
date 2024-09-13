using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 1)]
public sealed class ChatInputApplicationCommand : 
    ApplicationCommand,
    IChatInputApplicationCommandModel
{
    [JsonPropertyName("options")]
    public Optional<ApplicationCommandOption[]> Options { get; set; }

    IReadOnlyCollection<IApplicationCommandOptionModel>? IChatInputApplicationCommandModel.Options => ~Options;
}