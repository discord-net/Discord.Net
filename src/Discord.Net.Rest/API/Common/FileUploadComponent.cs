using Newtonsoft.Json;

namespace Discord.API;

public class FileUploadComponent : IInteractableComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("custom_id")]
    public string CustomId { get; set; }

    [JsonProperty("min_values")]
    public Optional<int> MinValues { get; set; }

    [JsonProperty("max_values")]
    public Optional<int> MaxValues { get; set; }

    [JsonProperty("required")]
    public Optional<bool> IsRequired { get; set; }

    [JsonProperty("values")]
    public Optional<string[]> Values { get; set; }

    public FileUploadComponent() {}

    public FileUploadComponent(Discord.FileUploadComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        CustomId = component.CustomId;
        MinValues = component.MinValues ?? Optional<int>.Unspecified;
        MaxValues = component.MaxValues ?? Optional<int>.Unspecified;
        IsRequired = component.IsRequired;
    }

    [JsonIgnore]
    int? IMessageComponent.Id => Id.ToNullable();
    IMessageComponentBuilder IMessageComponent.ToBuilder() => null;
}
