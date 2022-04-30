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

        internal new static RestUserCommand Create(DiscordRestClient client, Model model)
        {
            var entity = new RestUserCommand(client, model);
            entity.Update(client, model);
            return entity;
        }

        internal override void Update(DiscordRestClient client, Model model)
        {
            base.Update(client, model);

            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            Data = RestUserCommandData.Create(client, dataModel, Guild, Channel);
        }

        //IUserCommandInteractionData
        /// <inheritdoc/>
        IUserCommandInteractionData IUserCommandInteraction.Data => Data;

        //IApplicationCommandInteraction
        /// <inheritdoc/>
        IApplicationCommandInteractionData IApplicationCommandInteraction.Data => Data;
    }
}
