#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Relationship
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("type")]
        public RelationshipType Type { get; set; }
    }
}
