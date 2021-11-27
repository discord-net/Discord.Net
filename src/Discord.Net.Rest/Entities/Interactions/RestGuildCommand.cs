using System;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based guild application command.
    /// </summary>
    public class RestGuildCommand : RestApplicationCommand
    {
        /// <summary>
        ///     The guild Id where this command originates.
        /// </summary>
        public ulong GuildId { get; private set; }

        internal RestGuildCommand(BaseDiscordClient client, ulong id, ulong guildId)
            : base(client, id)
        {
            GuildId = guildId;
        }

        internal static RestGuildCommand Create(BaseDiscordClient client, Model model, ulong guildId)
        {
            var entity = new RestGuildCommand(client, model.Id, guildId);
            entity.Update(model);
            return entity;
        }

        /// <inheritdoc/>
        public override async Task DeleteAsync(RequestOptions options = null)
            => await InteractionHelper.DeleteGuildCommandAsync(Discord, GuildId, this).ConfigureAwait(false);

        /// <summary>
        ///     Modifies this <see cref="RestApplicationCommand"/>.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the command with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     The modified command
        /// </returns>
        public override async Task ModifyAsync<TArg>(Action<TArg> func, RequestOptions options = null)
        {
            var model = await InteractionHelper.ModifyGuildCommandAsync(Discord, this, GuildId, func, options).ConfigureAwait(false);
            Update(model);
        }

        /// <summary>
        ///     Gets this commands permissions inside of the current guild.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a
        ///     <see cref="GuildApplicationCommandPermission"/> object defining the permissions of the current slash command.
        /// </returns>
        public Task<GuildApplicationCommandPermission> GetCommandPermission(RequestOptions options = null)
            => InteractionHelper.GetGuildCommandPermissionAsync(Discord, GuildId, Id, options);

        /// <summary>
        ///     Modifies the current command permissions for this guild command.
        /// </summary>
        /// <param name="permissions">The permissions to overwrite.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///      A task that represents the asynchronous modification operation. The task result contains a
        ///     <see cref="GuildApplicationCommandPermission"/> object containing the modified permissions.
        /// </returns>
        public Task<GuildApplicationCommandPermission> ModifyCommandPermissions(ApplicationCommandPermission[] permissions, RequestOptions options = null)
            => InteractionHelper.ModifyGuildCommandPermissionsAsync(Discord, GuildId, Id, permissions, options);

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
            => ClientHelper.GetGuildAsync(Discord, GuildId, withCounts, options);
    }
}
