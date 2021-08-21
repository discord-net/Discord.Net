using System.Collections.Generic;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data tied with the <see cref="SocketUserCommand"/> interaction.
    /// </summary>
    public class SocketUserCommandData : SocketCommandBaseData
    {
        /// <summary>
        ///     The user used to run the command
        /// </summary>
        public SocketUser Member { get; private set; }

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>Note</b> Not implemented for <see cref="SocketUserCommandData"/>
        /// </remarks>
        public override IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options
            => throw new System.NotImplementedException();

        internal SocketUserCommandData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model, guildId) { }

        internal new static SocketUserCommandData Create(DiscordSocketClient client, Model model, ulong id, ulong? guildId)
        {
            var entity = new SocketUserCommandData(client, model, guildId);
            entity.Update(model);
            return entity;
        }
    }
}
