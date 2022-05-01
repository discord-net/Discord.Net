using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Model = Discord.API.Interaction;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based user command.
    /// </summary>
    public class RestUserCommand : RestCommandBase, IUserCommandInteraction, IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data associated with this interaction.
        /// </summary>
        public new RestUserCommandData Data { get; private set; }

        internal RestUserCommand(DiscordRestClient client, Model model)
            : base(client, model)
        {
        }

        internal new static async Task<RestUserCommand> CreateAsync(DiscordRestClient client, Model model, bool doApiCall)
        {
            var entity = new RestUserCommand(client, model);
            await entity.UpdateAsync(client, model, doApiCall).ConfigureAwait(false);
            return entity;
        }

        internal override async Task UpdateAsync(DiscordRestClient client, Model model, bool doApiCall)
        {
            await base.UpdateAsync(client, model, doApiCall).ConfigureAwait(false);

            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            Data = await RestUserCommandData.CreateAsync(client, dataModel, Guild, Channel, doApiCall).ConfigureAwait(false);
        }

        //IUserCommandInteractionData
        /// <inheritdoc/>
        IUserCommandInteractionData IUserCommandInteraction.Data => Data;

        //IApplicationCommandInteraction
        /// <inheritdoc/>
        IApplicationCommandInteractionData IApplicationCommandInteraction.Data => Data;
    }
}
