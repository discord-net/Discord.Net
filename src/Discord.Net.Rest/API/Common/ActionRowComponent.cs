using Newtonsoft.Json;
using System.Linq;

namespace Discord.API
{
    internal class ActionRowComponent : IMessageComponent, IMessageComponentModel
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("components")]
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
                    ComponentType.TextInput => new TextInputComponent(x as Discord.TextInputComponent),
                    _ => null
                };
            }).ToArray();
        }

        [JsonIgnore]
        string IMessageComponent.CustomId => null;

        ComponentType IMessageComponentModel.Type { get => Type; set => throw new System.NotSupportedException(); }
        IMessageComponentModel[] IMessageComponentModel.Components { get => Components.Select(x => x as IMessageComponentModel).ToArray(); set => throw new System.NotSupportedException(); } // cursed hack here

        #region unused
        string IMessageComponentModel.CustomId { get => null; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.Disabled { get => null; set => throw new System.NotSupportedException(); }
        ButtonStyle? IMessageComponentModel.Style { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Label { get => null; set => throw new System.NotSupportedException(); }
        ulong? IMessageComponentModel.EmojiId { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.EmojiName { get => null; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.EmojiAnimated { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Url { get => null; set => throw new System.NotSupportedException(); }
        IMessageComponentOptionModel[] IMessageComponentModel.Options { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Placeholder { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MinValues { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MaxValues { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MinLength { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MaxLength { get => null; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.Required { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Value { get => null; set => throw new System.NotSupportedException(); }
        #endregion
    }
}
