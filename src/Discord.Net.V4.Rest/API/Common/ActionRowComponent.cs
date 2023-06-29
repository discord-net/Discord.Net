using System.Text.Json.Serialization;

namespace Discord.API;

internal class ActionRowComponent : IMessageComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("components")]
    public IMessageComponent[] Components { get; set; }

    internal ActionRowComponent() { }
    internal ActionRowComponent(Discord.ActionRowComponent c)
    {
        Type = c.Type;
        Components = c.Components?.Select<IMessageComponent, IMessageComponent>(x =>
        {
            return x.Type switch
            {
                ComponentType.Button => new ButtonComponent((Discord.ButtonComponent)x),
                ComponentType.SelectMenu => new SelectMenuComponent((Discord.SelectMenuComponent)x),
                ComponentType.ChannelSelect => new SelectMenuComponent((Discord.SelectMenuComponent)x),
                ComponentType.UserSelect => new SelectMenuComponent((Discord.SelectMenuComponent)x),
                ComponentType.RoleSelect => new SelectMenuComponent((Discord.SelectMenuComponent)x),
                ComponentType.MentionableSelect => new SelectMenuComponent((Discord.SelectMenuComponent)x),
                ComponentType.TextInput => new TextInputComponent((Discord.TextInputComponent)x),
                _ => null!
            };
        }).ToArray() ?? Array.Empty<IMessageComponent>();
    }

    [JsonIgnore]
    string IMessageComponent.CustomId => null!;
}
