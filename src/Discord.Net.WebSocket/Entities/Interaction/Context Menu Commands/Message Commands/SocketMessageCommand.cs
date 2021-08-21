using Discord.Rest;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a Websocket-based slash command received over the gateway.
    /// </summary>
    public class SocketMessageCommand : SocketCommandBase
    {
        /// <summary>
        ///     The data associated with this interaction.
        /// </summary>
        new public SocketMessageCommandData Data { get; }

        internal SocketMessageCommand(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
            : base(client, model, channel)
        {
            var dataModel = model.Data.IsSpecified ?
                (DataModel)model.Data.Value
                : null;

            ulong? guildId = null;
            if (this.Channel is SocketGuildChannel guildChannel)
                guildId = guildChannel.Guild.Id;

            Data = SocketMessageCommandData.Create(client, dataModel, model.Id, guildId);
        }

        new internal static SocketInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel)
        {
            var entity = new SocketMessageCommand(client, model, channel);
            entity.Update(model);
            return entity;
        }
    }
}
