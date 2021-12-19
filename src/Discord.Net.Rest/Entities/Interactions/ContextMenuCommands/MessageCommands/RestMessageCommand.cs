using System.Threading.Tasks;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Model = Discord.API.Interaction;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based message command interaction.
    /// </summary>
    public class RestMessageCommand : RestCommandBase, IMessageCommandInteraction, IDiscordInteraction
    {
        /// <summary>
        ///     The data associated with this interaction.
        /// </summary>
        public new RestMessageCommandData Data { get; private set; }

        internal RestMessageCommand(DiscordRestClient client, Model model)
            : base(client, model)
        {
            
        }

        internal new static async Task<RestMessageCommand> CreateAsync(DiscordRestClient client, Model model)
        {
            var entity = new RestMessageCommand(client, model);
            await entity.UpdateAsync(client, model).ConfigureAwait(false);
            return entity;
        }

        internal override async Task UpdateAsync(DiscordRestClient client, Model model)
        {
            await base.UpdateAsync(client, model).ConfigureAwait(false);

            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;
            
            Data = await RestMessageCommandData.CreateAsync(client, dataModel, Guild, Channel).ConfigureAwait(false);
        }

        //IMessageCommandInteraction
        /// <inheritdoc/>
        IMessageCommandInteractionData IMessageCommandInteraction.Data => Data;
        //IApplicationCommandInteraction
        /// <inheritdoc/>
        IApplicationCommandInteractionData IApplicationCommandInteraction.Data => Data;
    }
}
