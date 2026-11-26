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

    public MenuActionItemComponent(Discord.MenuActionItemComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        CustomId = component.CustomId ?? Optional<string>.Unspecified;
        Label = component.Label;
        Style = component.Style;
        Description = component.Description ?? Optional<string>.Unspecified;
        Url = component.Url ?? Optional<string>.Unspecified;
        IsDisabled = component.IsDisabled;

        if (component.Emote != null)
        {
            if (component.Emote is Emote e)
            {
                Emoji = new Emoji
                {
                    Name = e.Name,
                    Animated = e.Animated,
                    Id = e.Id
                };
            }
            else
            {
                Emoji = new Emoji
                {
                    Name = component.Emote.Name
                };
            }
        }
    }

    [JsonIgnore]
    string IInteractableComponent.CustomId => CustomId.GetValueOrDefault();

    [JsonIgnore]
    int? IMessageComponent.Id => Id.ToNullable();

    IMessageComponentBuilder IMessageComponent.ToBuilder() => null;
}
