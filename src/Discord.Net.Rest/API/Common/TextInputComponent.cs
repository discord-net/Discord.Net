using Newtonsoft.Json;

namespace Discord.API
{
    internal class TextInputComponent : IMessageComponent, IMessageComponentModel
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("style")]
        public TextInputStyle Style { get; set; }

        [JsonProperty("custom_id")]
        public string CustomId { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("placeholder")]
        public Optional<string> Placeholder { get; set; }

        [JsonProperty("min_length")]
        public Optional<int> MinLength { get; set; }

        [JsonProperty("max_length")]
        public Optional<int> MaxLength { get; set; }

        [JsonProperty("value")]
        public Optional<string> Value { get; set; }

        [JsonProperty("required")]
        public Optional<bool> Required { get; set; }

        public TextInputComponent() { }

        public TextInputComponent(Discord.TextInputComponent component)
        {
            Type = component.Type;
            Style = component.Style;
            CustomId = component.CustomId;
            Label = component.Label;
            Placeholder = component.Placeholder;
            MinLength = component.MinLength ?? Optional<int>.Unspecified;
            MaxLength = component.MaxLength ?? Optional<int>.Unspecified;
            Required = component.Required ?? Optional<bool>.Unspecified;
            Value = component.Value ?? Optional<string>.Unspecified;
        }

        ComponentType IMessageComponentModel.Type { get => Type; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.CustomId { get => CustomId; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MinLength { get => MinLength.ToNullable(); set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MaxLength { get => MaxLength.ToNullable(); set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.Required { get => Required.ToNullable(); set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Value { get => Value.GetValueOrDefault(); set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Label { get => Label; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Placeholder { get => Placeholder.GetValueOrDefault(); set => throw new System.NotSupportedException(); }

        #region unused

        bool? IMessageComponentModel.Disabled { get => null; set => throw new System.NotSupportedException(); }
        ButtonStyle? IMessageComponentModel.Style { get => null; set => throw new System.NotSupportedException(); }
        ulong? IMessageComponentModel.EmojiId { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.EmojiName { get => null; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.EmojiAnimated { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Url { get => null; set => throw new System.NotSupportedException(); }
        IMessageComponentOptionModel[] IMessageComponentModel.Options { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MinValues { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MaxValues { get => null; set => throw new System.NotSupportedException(); }
        IMessageComponentModel[] IMessageComponentModel.Components { get => null; set => throw new System.NotSupportedException(); }

        #endregion
    }
}
