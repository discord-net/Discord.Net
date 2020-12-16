using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class ApplicationCommandParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("options")]
        public Optional<ApplicationCommandOption[]> Options { get; set; }

        public ApplicationCommandParams() { }
        public ApplicationCommandParams(string name, string description, ApplicationCommandOption[] options = null)
        {
            this.Name = name;
            this.Description = description;
            this.Options = Optional.Create<ApplicationCommandOption[]>(options);
        }
    }
}
