using Newtonsoft.Json;

namespace Discord.API
{
    internal class SelectMenuOption : IMessageComponentOptionModel
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("description")]
        public Optional<string> Description { get; set; }

        [JsonProperty("emoji")]
        public Optional<Emoji> Emoji { get; set; }

        [JsonProperty("default")]
        public Optional<bool> Default { get; set; }

        public SelectMenuOption() { }

        public SelectMenuOption(Discord.SelectMenuOption option)
        {
            Label = option.Label;
            Value = option.Value;
            Description = option.Description;

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

        string IMessageComponentOptionModel.Label { get => Label; set => throw new System.NotSupportedException(); }
        string IMessageComponentOptionModel.Value { get => Value; set => throw new System.NotSupportedException(); }
        string IMessageComponentOptionModel.Description { get => Description.GetValueOrDefault(); set => throw new System.NotSupportedException(); }
        ulong? IMessageComponentOptionModel.EmojiId { get => Emoji.GetValueOrDefault()?.Id; set => throw new System.NotSupportedException(); }
        string IMessageComponentOptionModel.EmojiName { get => Emoji.GetValueOrDefault()?.Name; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentOptionModel.EmojiAnimated { get => Emoji.GetValueOrDefault()?.Animated; set => throw new System.NotSupportedException(); }
        bool? IMessageComponentOptionModel.Default { get => Default.ToNullable(); set => throw new System.NotSupportedException(); }
    }
}
