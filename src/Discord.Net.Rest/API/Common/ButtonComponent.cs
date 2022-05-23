using Newtonsoft.Json;

namespace Discord.API
{
    internal class ButtonComponent : IMessageComponent, IMessageComponentModel
    {
        [JsonProperty("type")]
        public ComponentType Type { get; set; }

        [JsonProperty("style")]
        public ButtonStyle Style { get; set; }

        [JsonProperty("label")]
        public Optional<string> Label { get; set; }

        [JsonProperty("emoji")]
        public Optional<Emoji> Emote { get; set; }

        [JsonProperty("custom_id")]
        public Optional<string> CustomId { get; set; }

        [JsonProperty("url")]
        public Optional<string> Url { get; set; }

        [JsonProperty("disabled")]
        public Optional<bool> Disabled { get; set; }

        public ButtonComponent() { }

        public ButtonComponent(Discord.ButtonComponent c)
        {
            Type = c.Type;
            Style = c.Style;
            Label = c.Label;
            CustomId = c.CustomId;
            Url = c.Url;
            Disabled = c.IsDisabled;

            if (c.Emote != null)
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

        ComponentType IMessageComponentModel.Type { get => Type; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.CustomId { get => CustomId.GetValueOrDefault(); set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.Disabled { get => Disabled.ToNullable(); set => throw new System.NotSupportedException(); }
        ButtonStyle? IMessageComponentModel.Style { get => Style; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Label { get => Label.GetValueOrDefault(); set => throw new System.NotSupportedException(); }
        ulong? IMessageComponentModel.EmojiId { get => Emote.GetValueOrDefault()?.Id; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.EmojiName { get => Emote.GetValueOrDefault()?.Name; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.EmojiAnimated { get => Emote.GetValueOrDefault()?.Animated; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Url { get => Url.GetValueOrDefault(); set => throw new System.NotSupportedException(); }

        #region unused
        IMessageComponentOptionModel[] IMessageComponentModel.Options { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Placeholder { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MinValues { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MaxValues { get => null; set => throw new System.NotSupportedException(); }
        IMessageComponentModel[] IMessageComponentModel.Components { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MinLength { get => null; set => throw new System.NotSupportedException(); }
        int? IMessageComponentModel.MaxLength { get => null; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentModel.Required { get => null; set => throw new System.NotSupportedException(); }
        string IMessageComponentModel.Value { get => null; set => throw new System.NotSupportedException(); }
        #endregion
    }
}
