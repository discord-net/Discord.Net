using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommand;

namespace Discord.Rest
{
    public class RestGlobalUserCommand : RestApplicationCommand
    {
        internal RestGlobalUserCommand(BaseDiscordClient client, ulong id)
            : base(client, id)
        {
            this.CommandType = RestApplicationCommandType.GlobalUserCommand;
        }

        internal static RestGlobalUserCommand Create(BaseDiscordClient client, Model model)
        {
            var entity = new RestGlobalUserCommand(client, model.Id);
            entity.Update(model);
            return entity;
        }

        /// <inheritdoc/>
        public override async Task DeleteAsync(RequestOptions options = null)
            => await InteractionHelper.DeleteGlobalUserCommand(Discord, this).ConfigureAwait(false);

        /// <summary>
        ///     Modifies this <see cref="RestApplicationCommand"/>.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the command with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     The modified command.
        /// </returns>
        public async Task<RestGlobalUserCommand> ModifyAsync(Action<ApplicationCommandProperties> func, RequestOptions options = null)
            => await InteractionHelper.ModifyGlobalUserCommand(Discord, this, func, options).ConfigureAwait(false);
    }
}
