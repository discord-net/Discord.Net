using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    public class SocketSlashCommandData : SocketEntity<ulong>, IApplicationCommandInteractionData
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <summary>
        ///     The <see cref="SocketSlashCommandDataOption"/>'s received with this interaction.
        /// </summary>
        public IReadOnlyCollection<SocketSlashCommandDataOption> Options { get; private set; }

        private ulong guildId;

        internal SocketSlashCommandData(DiscordSocketClient client, ulong id)
            : base(client, id)
        {

        }

        internal static SocketSlashCommandData Create(DiscordSocketClient client, Model model, ulong guildId)
        {
            var entity = new SocketSlashCommandData(client, model.Id);
            entity.Update(model, guildId);
            return entity;
        }
        internal void Update(Model model, ulong guildId)
        {
            this.Name = model.Name;
            this.guildId = guildId;

            this.Options = model.Options.Any()
                ? model.Options.Select(x => new SocketSlashCommandDataOption(x, this.Discord, guildId)).ToImmutableArray()
                : null;
        }

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionData.Options => Options;
    }
}
