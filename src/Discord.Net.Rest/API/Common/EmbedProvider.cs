#pragma warning disable CS1591
using System;
using Discord.Serialization;

namespace Discord.API
{
    internal class EmbedProvider
    {
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("url")]
        public string Url { get; set; }
    }
}
