using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.API.Rest
{
    internal class CreateApplicationCommandParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public ApplicationCommandType Type { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonProperty("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonProperty("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonProperty("description_localizations")]
        public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }

        [JsonProperty("dm_permission")]
        public Optional<bool?> DmPermission { get; set; }

        [JsonProperty("default_member_permissions")]
        public Optional<GuildPermission?> DefaultMemberPermission { get; set; }

        [JsonProperty("nsfw")]
        public Optional<bool> Nsfw { get; set; }

        [JsonProperty("contexts")]
        public Optional<HashSet<InteractionContextType>> ContextTypes { get; set; }

        [JsonProperty("integration_types")]
        public Optional<HashSet<ApplicationIntegrationType>> IntegrationTypes { get; set; }

        public CreateApplicationCommandParams() { }

        public CreateApplicationCommandParams(string name, string description, ApplicationCommandType type, ApplicationCommandOption[] options = null,
            IDictionary<string, string> nameLocalizations = null, IDictionary<string, string> descriptionLocalizations = null, bool nsfw = false,
            HashSet<InteractionContextType> contextTypes = null, HashSet<ApplicationIntegrationType> integrationTypes = null)
        {
            Name = name;
            Description = description;
            Options = Optional.Create(options);
            Type = type;
            NameLocalizations = nameLocalizations?.ToDictionary(x => x.Key, x => x.Value) ?? Optional<Dictionary<string, string>>.Unspecified;
            DescriptionLocalizations = descriptionLocalizations?.ToDictionary(x => x.Key, x => x.Value) ?? Optional<Dictionary<string, string>>.Unspecified;
            Nsfw = nsfw;
            ContextTypes = contextTypes ?? Optional.Create<HashSet<InteractionContextType>>();
            IntegrationTypes = integrationTypes ?? Optional.Create<HashSet<ApplicationIntegrationType>>();
        }
    }
}
