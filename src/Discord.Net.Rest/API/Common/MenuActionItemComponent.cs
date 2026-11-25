using Newtonsoft.Json;

namespace Discord.API;

internal class MenuActionItemComponent : IInteractableComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("custom_id")]
    public Optional<string> CustomId { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("style")]
    public MenuActionItemStyle Style { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("url")]
    public Optional<string> Url { get; set; }

    [JsonProperty("emoji")]
    public Optional<Emoji> Emoji { get; set; }

    [JsonProperty("disabled")]
    public Optional<bool> IsDisabled { get; set; }


    public MenuActionItemComponent() { }

    [JsonIgnore]
    string IInteractableComponent.CustomId => CustomId.GetValueOrDefault();

    [JsonIgnore]
    int? IMessageComponent.Id => Id.ToNullable();

    IMessageComponentBuilder IMessageComponent.ToBuilder() => null;
}
