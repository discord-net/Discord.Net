using System.Text.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.API.Rest
{
    internal class CreateApplicationCommandParams
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public ApplicationCommandType Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonPropertyName("default_permission")]
        public Optional<bool> DefaultPermission { get; set; }

        [JsonPropertyName("name_localizations")]
        public Optional<Dictionary<string, string>> NameLocalizations { get; set; }

        [JsonPropertyName("description_localizations")]
        public Optional<Dictionary<string, string>> DescriptionLocalizations { get; set; }

        [JsonPropertyName("dm_permission")]
        public Optional<bool?> DmPermission { get; set; }

        [JsonPropertyName("default_member_permissions")]
        public Optional<GuildPermission?> DefaultMemberPermission { get; set; }

        public CreateApplicationCommandParams() { }
        public CreateApplicationCommandParams(string name, string description, ApplicationCommandType type, ApplicationCommandOption[] options = null,
            IDictionary<string, string> nameLocalizations = null, IDictionary<string, string> descriptionLocalizations = null)
        {
            Name = name;
            Description = description;
            Options = Optional.Create(options);
            Type = type;
            NameLocalizations = nameLocalizations?.ToDictionary(x => x.Key, x => x.Value) ?? Optional<Dictionary<string, string>>.Unspecified;
            DescriptionLocalizations = descriptionLocalizations?.ToDictionary(x => x.Key, x => x.Value) ?? Optional<Dictionary<string, string>>.Unspecified;
        }
    }
}
