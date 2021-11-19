using System.Collections.Generic;
using System.Linq;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data tied with the <see cref="SocketMessageCommand"/> interaction.
    /// </summary>
    public class SocketMessageCommandData : SocketCommandBaseData, IMessageCommandInteractionData, IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the message associated with this message command.
        /// </summary>
        public SocketMessage Message
            => ResolvableData?.Messages.FirstOrDefault().Value;

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>Note</b> Not implemented for <see cref="SocketMessageCommandData"/>
        /// </remarks>
        public override IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options
            => throw new System.NotImplementedException();

        internal SocketMessageCommandData(DiscordSocketClient client, Model model, ulong? guildId)
            : base(client, model, guildId) { }

        internal new static SocketMessageCommandData Create(DiscordSocketClient client, Model model, ulong id, ulong? guildId)
        {
            var entity = new SocketMessageCommandData(client, model, guildId);
            entity.Update(model);
            return entity;
        }

        //IMessageCommandInteractionData
        /// <inheritdoc/>
        IMessage IMessageCommandInteractionData.Message => Message;
    }
}
