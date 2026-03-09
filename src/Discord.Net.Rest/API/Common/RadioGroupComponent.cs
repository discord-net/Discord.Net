using Newtonsoft.Json;
using System.Linq;

namespace Discord.API;

internal class RadioGroupComponent : IInteractableComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("custom_id")]
    public string CustomId { get; set; }

    [JsonProperty("options")]
    public RadioGroupOption[] Options { get; set; }

    [JsonProperty("required")]
    public Optional<bool> IsRequired { get; set; }

    [JsonProperty("value")]
    public Optional<string> Value { get; set; } 

    public RadioGroupComponent() { }

    public RadioGroupComponent(Discord.RadioGroupComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        CustomId = component.CustomId;
        Options = component.Options.Select(x => new RadioGroupOption
        {
            Description = x.Description,
            IsDefault = x.IsDefault,
            Label = x.Label,
            Value = x.Value,
        }).ToArray();
        IsRequired = component.IsRequired ?? Optional<bool>.Unspecified;;
    }

    [JsonIgnore]
    int? IMessageComponent.Id => Id.ToNullable();
    IMessageComponentBuilder IMessageComponent.ToBuilder() => null;
}
