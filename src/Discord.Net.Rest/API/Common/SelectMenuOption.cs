using Newtonsoft.Json;

namespace Discord.API
{
    internal class SelectMenuOption
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
    }
}
