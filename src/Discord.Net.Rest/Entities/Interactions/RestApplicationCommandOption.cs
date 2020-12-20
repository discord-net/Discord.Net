using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandOption;

namespace Discord.Rest
{
    public class RestApplicationCommandOption : IApplicationCommandOption
    {
        public ApplicationCommandOptionType Type { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public bool? Default { get; private set; }

        public bool? Required { get; private set; }

        public IReadOnlyCollection<RestApplicationCommandChoice> Choices { get; private set; }

        public IReadOnlyCollection<RestApplicationCommandOption> Options { get; private set; }

        internal RestApplicationCommandOption() { }

        internal static RestApplicationCommandOption Create(Model model)
        {
            var options = new RestApplicationCommandOption();
            options.Update(model);
            return options;
        }

        internal void Update(Model model)
        {
            this.Type = model.Type;
            this.Name = model.Name;
            this.Description = model.Description;

            if (model.Default.IsSpecified)
                this.Default = model.Default.Value;

            if (model.Required.IsSpecified)
                this.Required = model.Required.Value;

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => Create(x)).ToImmutableArray()
                : null;

            this.Choices = model.Choices.IsSpecified
                ? model.Choices.Value.Select(x => new RestApplicationCommandChoice(x)).ToImmutableArray()
                : null;
        }

        IReadOnlyCollection<IApplicationCommandOption> IApplicationCommandOption.Options => Options;
        IReadOnlyCollection<IApplicationCommandOptionChoice> IApplicationCommandOption.Choices => Choices;
    }
}
