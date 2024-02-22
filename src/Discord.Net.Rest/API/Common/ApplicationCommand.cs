using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class ApplicationCommand
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("type")]
        public ApplicationCommandType Type { get; set; } = ApplicationCommandType.Slash; // defaults to 1 which is slash.

        [JsonProperty("application_id")]
        public ulong ApplicationId { get; set; }

        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonProperty("default_permission")]
        public Optional<bool> DefaultPermissions { get; set; }

        [JsonProperty("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonProperty("description_localizations")]
        public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }

        [JsonProperty("name_localized")]
        public Optional<string> NameLocalized { get; set; }

        [JsonProperty("description_localized")]
        public Optional<string> DescriptionLocalized { get; set; }

        // V2 Permissions
        [JsonProperty("dm_permission")]
        public Optional<bool?> DmPermission { get; set; }

        [JsonProperty("default_member_permissions")]
        public Optional<GuildPermission?> DefaultMemberPermission { get; set; }

        [JsonProperty("nsfw")]
        public Optional<bool?> Nsfw { get; set; }

        [JsonProperty("contexts")]
        public Optional<InteractionContextType[]> ContextTypes { get; set; }

        [JsonProperty("integration_types")]
        public Optional<ApplicationIntegrationType[]> IntegrationTypes { get; set; }
    }
}
