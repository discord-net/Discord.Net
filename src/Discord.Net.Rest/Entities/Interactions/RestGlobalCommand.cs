using System;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based global application command.
    /// </summary>
    public class RestGlobalCommand : RestApplicationCommand
    {
        internal RestGlobalCommand(BaseDiscordClient client, ulong id)
            : base(client, id) { }

        internal static RestGlobalCommand Create(BaseDiscordClient client, Model model)
        {
            var entity = new RestGlobalCommand(client, model.Id);
            entity.Update(model);
            return entity;
        }

        /// <inheritdoc/>
        public override async Task DeleteAsync(RequestOptions options = null)
            => await InteractionHelper.DeleteGlobalCommandAsync(Discord, this).ConfigureAwait(false);

        /// <summary>
        ///     Modifies this <see cref="RestApplicationCommand"/>.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the command with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     The modified command.
        /// </returns>
        public override async Task ModifyAsync<TArg>(Action<TArg> func, RequestOptions options = null)
        {
            var cmd = await InteractionHelper.ModifyGlobalCommandAsync(Discord, this, func, options).ConfigureAwait(false);
            Update(cmd);
        }
    }
}
