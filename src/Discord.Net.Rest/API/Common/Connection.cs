#pragma warning disable CS1591
using Discord.Serialization;
using System.Collections.Generic;

namespace Discord.API
{
    internal class Connection
    {
        [ModelProperty("id")]
        public string Id { get; set; }
        [ModelProperty("type")]
        public string Type { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("revoked")]
        public bool Revoked { get; set; }

        [ModelProperty("integrations")]
        public IReadOnlyCollection<ulong> Integrations { get; set; }
    }
}
