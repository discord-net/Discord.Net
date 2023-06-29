using System.Text.Json.Serialization;

namespace Discord.API;

internal class ButtonComponent : IMessageComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("style")]
    public ButtonStyle Style { get; set; }

    [JsonPropertyName("label")]
    public Optional<string> Label { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<Emoji> Emote { get; set; }

    [JsonPropertyName("custom_id")]
    public Optional<string> CustomId { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    [JsonPropertyName("disabled")]
    public Optional<bool> Disabled { get; set; }

    public ButtonComponent() { }

    public ButtonComponent(Discord.ButtonComponent c)
    {
        Type = c.Type;
        Style = c.Style;
        Label = c.Label ?? Optional<string>.Unspecified;
        CustomId = c.CustomId ?? Optional<string>.Unspecified;
        Url = c.Url ?? Optional<string>.Unspecified;
        Disabled = c.IsDisabled;

        if (c.Emote is not null)
        {
            if (c.Emote is Emote e)
            {
                Emote = new Emoji
                {
                    Name = e.Name,
                    Animated = e.Animated,
                    Id = e.Id
                };
            }
            else
            {
                Emote = new Emoji
                {
                    Name = c.Emote.Name
                };
            }
        }
    }

    [JsonIgnore]
    string IMessageComponent.CustomId => CustomId.GetValueOrDefault();
}
