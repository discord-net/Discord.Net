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
        ///     Gets the data associated with this interaction.
        /// </summary>
        public new RestMessageCommandData Data { get; private set; }

        internal RestMessageCommand(DiscordRestClient client, Model model)
            : base(client, model)
        {
            
        }

        internal new static RestMessageCommand Create(DiscordRestClient client, Model model)
        {
            var entity = new RestMessageCommand(client, model);
            entity.Update(client, model);
            return entity;
        }

        internal override void Update(DiscordRestClient client, Model model)
        {
            base.Update(client, model);

            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;
            
            Data = RestMessageCommandData.Create(client, dataModel, Guild, Channel);
        }

        //IMessageCommandInteraction
        /// <inheritdoc/>
        IMessageCommandInteractionData IMessageCommandInteraction.Data => Data;
        //IApplicationCommandInteraction
        /// <inheritdoc/>
        IApplicationCommandInteractionData IApplicationCommandInteraction.Data => Data;
    }
}
