using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket.Entities.Interaction
{
    public class SocketInteractionData : SocketEntity<ulong>, IApplicationCommandInteractionData
    {
        public string Name { get; private set; }
        public IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options { get; private set; }

        internal SocketInteractionData(DiscordSocketClient client, ulong id)
            : base(client, id)
        {

        }

        internal static SocketInteractionData Create(DiscordSocketClient client, Model model)
        {
            var entity = new SocketInteractionData(client, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            this.Name = model.Name;
            this.Options = model.Options.IsSpecified
                ? model.Options.Value.Select(x => new SocketInteractionDataOption(x)).ToImmutableArray()
                : null;

        }
    }
}
