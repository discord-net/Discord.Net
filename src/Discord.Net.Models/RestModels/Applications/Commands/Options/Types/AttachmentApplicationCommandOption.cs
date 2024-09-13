using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), 11)]
public sealed class AttachmentApplicationCommandOption : 
    ApplicationCommandOption, 
    IAttachmentApplicationCommandOptionModel
{
    [JsonPropertyName("required")]
    public Optional<bool> IsRequired { get; set; }

    bool? IAttachmentApplicationCommandOptionModel.IsRequired => IsRequired.ToNullable();
}