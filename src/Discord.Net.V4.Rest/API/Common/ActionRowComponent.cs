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
                ComponentType.Button => new ButtonComponent(x as Discord.ButtonComponent),
                ComponentType.SelectMenu => new SelectMenuComponent(x as Discord.SelectMenuComponent),
                ComponentType.ChannelSelect => new SelectMenuComponent(x as Discord.SelectMenuComponent),
                ComponentType.UserSelect => new SelectMenuComponent(x as Discord.SelectMenuComponent),
                ComponentType.RoleSelect => new SelectMenuComponent(x as Discord.SelectMenuComponent),
                ComponentType.MentionableSelect => new SelectMenuComponent(x as Discord.SelectMenuComponent),
                ComponentType.TextInput => new TextInputComponent(x as Discord.TextInputComponent),
                _ => null
            };
        }).ToArray();
    }

    [JsonIgnore]
    string IMessageComponent.CustomId => null;
}