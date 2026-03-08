using Newtonsoft.Json;

namespace Discord.API;

internal class CheckboxComponent : IInteractableComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("custom_id")]
    public string CustomId { get; set; }

    [JsonProperty("default")]
    public Optional<bool> DefaultState { get; set; }

    [JsonProperty("value")]
    public Optional<bool> Value { get; set; } 

    public CheckboxComponent() { }

    public CheckboxComponent(Discord.CheckboxComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        CustomId = component.CustomId;
        DefaultState = component.DefaultState;
    }

    [JsonIgnore]
    int? IMessageComponent.Id => Id.ToNullable();
    IMessageComponentBuilder IMessageComponent.ToBuilder() => null;
}
