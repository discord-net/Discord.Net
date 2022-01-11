using Newtonsoft.Json;

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

        public CreateApplicationCommandParams() { }
        public CreateApplicationCommandParams(string name, string description, ApplicationCommandType type, ApplicationCommandOption[] options = null)
        {
            Name = name;
            Description = description;
            Options = Optional.Create(options);
            Type = type;
        }
    }
}
