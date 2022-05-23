using Newtonsoft.Json;
using System.Linq;

namespace Discord.API
{
    internal class SelectMenuComponent : IMessageComponent, IMessageComponentModel
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("options")]
        public SelectMenuOption[] Options { get; set; }

        [JsonProperty("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonProperty("min_values")]
        public int MinValues { get; set; }

        [JsonProperty("max_values")]
        public int MaxValues { get; set; }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("values")]
        public Optional<string[]> Values { get; set; }

        public SelectMenuComponent() { }

        public SelectMenuComponent(Discord.SelectMenuComponent component)
        {
            Type = component.Type;
            CustomId = component.CustomId;
            Options = component.Options.Select(x => new SelectMenuOption(x)).ToArray();
            Placeholder = component.Placeholder;
            MinValues = component.MinValues;
            MaxValues = component.MaxValues;
            Disabled = component.IsDisabled;
        }

        ComponentType IMessageComponentModel.Type { get => Type; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.CustomId { get => CustomId; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.Disabled { get => Disabled; set => throw new System.NotSupportedException(); }
        IMessageComponentOptionModel[] IMessageComponentModel.Options { get => Options; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Placeholder { get => Placeholder.GetValueOrDefault(); set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MinValues { get => MinValues; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MaxValues { get => MaxValues; set => throw new System.NotSupportedException(); }

        #region unused
        ButtonStyle? IMessageComponentModel.Style { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Label { get => null; set => throw new System.NotSupportedException(); }
        ulong? IMessageComponentModel.EmojiId { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.EmojiName { get => null; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.EmojiAnimated { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Url { get => null; set => throw new System.NotSupportedException(); }
        IMessageComponentModel[] IMessageComponentModel.Components { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MinLength { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MaxLength { get => null; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.Required { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Value { get => null; set => throw new System.NotSupportedException(); }
        #endregion
    }
}
