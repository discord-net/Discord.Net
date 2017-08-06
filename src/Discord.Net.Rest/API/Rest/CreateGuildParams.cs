#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class CreateGuildParams
    {
        [ModelProperty("name")]
        public string Name { get; }
        [ModelProperty("region")]
        public string RegionId { get; }

        [ModelProperty("icon")]
        public Optional<Image?> Icon { get; set; }

        public CreateGuildParams(string name, string regionId)
        {
            Name = name;
            RegionId = regionId;
        }
    }
}
