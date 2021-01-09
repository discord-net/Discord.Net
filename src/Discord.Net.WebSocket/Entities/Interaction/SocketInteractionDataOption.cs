using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandInteractionDataOption;

namespace Discord.WebSocket
{
    public class SocketInteractionDataOption : IApplicationCommandInteractionDataOption
    {
        public string Name { get; private set; }
        public object? Value { get; private set; }

        public IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; private set; }

        internal SocketInteractionDataOption(Model model)
        {
            this.Name = Name;
            this.Value = model.Value.IsSpecified ? model.Value.Value : null;

            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new SocketInteractionDataOption(x)).ToImmutableArray()
                : null;
        }
    }
}
