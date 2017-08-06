#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class CreateGuildIntegrationParams
    {
        [ModelProperty("id")]
        public ulong Id { get; }
        [ModelProperty("type")]
        public string Type { get; }

        public CreateGuildIntegrationParams(ulong id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
