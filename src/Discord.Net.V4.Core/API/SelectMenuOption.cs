using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class SelectMenuOption
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<Emoji> Emoji { get; set; }

    [JsonPropertyName("default")]
    public Optional<bool> Default { get; set; }

    public SelectMenuOption() { }

    public SelectMenuOption(Discord.SelectMenuOption option)
    {
        Label = option.Label;
        Value = option.Value;
        Description = option.Description ?? Optional<string>.Unspecified;

        if (option.Emote != null)
        {
            if (option.Emote is Emote e)
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
                    Name = option.Emote.Name
                };
            }
        }

        Default = option.IsDefault ?? Optional<bool>.Unspecified;
    }
}
