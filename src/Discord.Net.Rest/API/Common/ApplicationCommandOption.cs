using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ApplicationCommandOption
    {
        [JsonProperty("type")]
        public ApplicationCommandOptionType Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("default")]
        public Optional<bool> Default { get; set; }

        [JsonProperty("required")]
        public Optional<bool> Required { get; set; }

        [JsonProperty("choices")]
        public Optional<ApplicationCommandOptionChoice[]> Choices { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        [JsonProperty("autocomplete")]
        public Optional<bool> Autocomplete { get; set; }

        [JsonProperty("channel_types")]
        public Optional<ChannelType[]> ChannelTypes { get; set; }

        public ApplicationCommandOption() { }

        public ApplicationCommandOption(IApplicationCommandOption cmd)
        {
            Choices = cmd.Choices.Select(x => new ApplicationCommandOptionChoice()
            {
                Name = x.Name,
                Value = x.Value
            }).ToArray();

            Options = cmd.Options.Select(x => new ApplicationCommandOption(x)).ToArray();

            ChannelTypes = cmd.ChannelTypes.ToArray();

            Required = cmd.IsRequired.HasValue
                ? cmd.IsRequired.Value
                : Optional<bool>.Unspecified;
            Default = cmd.IsDefault.HasValue
                ? cmd.IsDefault.Value
                : Optional<bool>.Unspecified;

            Name = cmd.Name;
            Type = cmd.Type;
            Description = cmd.Description;
        }
        public ApplicationCommandOption(Discord.ApplicationCommandOptionProperties option)
        {
            Choices = option.Choices != null
                ? option.Choices.Select(x => new ApplicationCommandOptionChoice()
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToArray()
                : Optional<ApplicationCommandOptionChoice[]>.Unspecified;

            Options = option.Options != null
                ? option.Options.Select(x => new ApplicationCommandOption(x)).ToArray()
                : Optional<ApplicationCommandOption[]>.Unspecified;

            Required = option.Required.HasValue
                ? option.Required.Value
                : Optional<bool>.Unspecified;

            Default = option.Default.HasValue
                ? option.Default.Value
                : Optional<bool>.Unspecified;

            ChannelTypes = option.ChannelTypes != null
                ? option.ChannelTypes.ToArray()
                : Optional<ChannelType[]>.Unspecified;

            Name = option.Name;
            Type = option.Type;
            Description = option.Description;
            Autocomplete = option.Autocomplete;
        }
    }
}
