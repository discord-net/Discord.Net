using DataModel = Discord.API.ApplicationCommandInteractionData;
using Model = Discord.API.Interaction;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a Websocket-based slash command received over the gateway.
    /// </summary>
    public class SocketSlashCommand : SocketCommandBase, ISlashCommandInteraction, IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data associated with this interaction.
        /// </summary>
        public new SocketSlashCommandData Data { get; }

        internal SocketSlashCommand(DiscordSocketClient client, Model model, ISocketMessageChannel channel, SocketUser user)
            : base(client, model, channel, user)
        {
            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            ulong? guildId = model.GuildId.ToNullable();

            Data = SocketSlashCommandData.Create(client, dataModel, guildId);
        }

        internal new static SocketInteraction Create(DiscordSocketClient client, Model model, ISocketMessageChannel channel, SocketUser user)
        {
            var entity = new SocketSlashCommand(client, model, channel, user);
            entity.Update(model);
            return entity;
        }

        //ISlashCommandInteraction
        /// <inheritdoc/>
        IApplicationCommandInteractionData ISlashCommandInteraction.Data => Data;

        //IDiscordInteraction
        /// <inheritdoc/>
        IDiscordInteractionData IDiscordInteraction.Data => Data;

        //IApplicationCommandInteraction
        /// <inheritdoc/>
        IApplicationCommandInteractionData IApplicationCommandInteraction.Data => Data;
    }
}
