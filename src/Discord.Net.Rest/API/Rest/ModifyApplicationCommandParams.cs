using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Rest
{
    internal class ModifyApplicationCommandParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }

        [JsonProperty("description")]
        public Optional<string> Description { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonProperty("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonProperty("nsfw")]
        public Optional<bool> Nsfw { get; set; }

        [JsonProperty("default_member_permissions")]
        public Optional<GuildPermission?> DefaultMemberPermission { get; set; }

        [JsonProperty("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonProperty("description_localizations")]
        public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }

        [JsonProperty("contexts")]
        public Optional<HashSet<InteractionContextType>> ContextTypes { get; set; }

        [JsonProperty("integration_types")]
        public Optional<HashSet<ApplicationIntegrationType>> IntegrationTypes { get; set; }
    }
}
