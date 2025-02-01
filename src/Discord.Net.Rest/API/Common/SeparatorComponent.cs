using Newtonsoft.Json;

namespace Discord.API;

internal class SeparatorComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("divider")]
    public Optional<bool> IsDivider { get; set; }

    [JsonProperty("spacing")]
    public Optional<SeparatorSpacingSize> Spacing { get; set; }

    public SeparatorComponent() { }
    int? IMessageComponent.Id => Id.ToNullable();
}
