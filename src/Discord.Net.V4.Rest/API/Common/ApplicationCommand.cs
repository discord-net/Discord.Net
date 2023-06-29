using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class ApplicationCommand
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("type")]
        public ApplicationCommandType Type { get; set; } = ApplicationCommandType.Slash; // defaults to 1 which is slash.

        [JsonPropertyName("application_id")]
        public ulong ApplicationId { get; set; }

        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonPropertyName("default_permission")]
        public Optional<bool> DefaultPermissions { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }

        [JsonPropertyName("name_localized")]
        public Optional<string> NameLocalized { get; set; }

        [JsonPropertyName("description_localized")]
        public Optional<string> DescriptionLocalized { get; set; }

        // V2 Permissions
        [JsonPropertyName("dm_permission")]
        public Optional<bool?> DmPermission { get; set; }

        [JsonPropertyName("default_member_permissions")]
        public Optional<GuildPermission?> DefaultMemberPermission { get; set; }

        [JsonPropertyName("nsfw")]
        public Optional<bool?> Nsfw { get; set; }
    }
}
