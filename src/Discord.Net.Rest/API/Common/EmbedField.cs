using Discord.Serialization;

namespace Discord.API
{
    internal class EmbedField
    {
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("value")]
        public string Value { get; set; }
        [ModelProperty("inline")]
        public bool Inline { get; set; }
    }
}
