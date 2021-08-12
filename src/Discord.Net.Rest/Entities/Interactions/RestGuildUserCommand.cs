using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based guild command.
    /// </summary>
    public class RestGuildUserCommand : RestApplicationCommand
    {
        /// <summary>
        ///     The guild Id where this command originates.
        /// </summary>
        public ulong GuildId { get; private set; }

        internal RestGuildUserCommand(BaseDiscordClient client, ulong id, ulong guildId)
            : base(client, id)
        {
            this.CommandType = RestApplicationCommandType.GuildUserCommand;
            this.GuildId = guildId;
        }

        internal static RestGuildUserCommand Create(BaseDiscordClient client, Model model, ulong guildId)
        {
            var entity = new RestGuildUserCommand(client, model.Id, guildId);
            entity.Update(model);
            return entity;
        }

        /// <inheritdoc/>
        public override async Task DeleteAsync(RequestOptions options = null)
            => await InteractionHelper.DeleteGuildUserCommand(Discord, GuildId, this).ConfigureAwait(false);

        /// <summary>
        ///     Modifies this <see cref="RestApplicationCommand"/>.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the command with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     The modified command
        /// </returns>
        public async Task<RestGuildUserCommand> ModifyAsync(Action<ApplicationCommandProperties> func, RequestOptions options = null)
            => await InteractionHelper.ModifyGuildUserCommand(Discord, this, func, options).ConfigureAwait(false);

        /// <summary>
        ///     Gets the guild that this slash command resides in.
        /// </summary>
        /// <param name="withCounts"><see langword="true"/> if you want the approximate member and presence counts for the guild, otherwise <see langword="false"/>.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a
        ///     <see cref="RestGuild"/>.
        /// </returns>
        public Task<RestGuild> GetGuild(bool withCounts = false, RequestOptions options = null)
            => ClientHelper.GetGuildAsync(this.Discord, this.GuildId, withCounts, options);
    }
}
