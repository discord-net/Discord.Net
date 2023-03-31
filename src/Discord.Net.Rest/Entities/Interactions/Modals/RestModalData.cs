using System;
using System.Collections.Generic;
using System.Linq;
using DataModel = Discord.API.MessageComponentInteractionData;
using InterationModel = Discord.API.Interaction;
using Model = Discord.API.ModalInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents data sent from a <see cref="InteractionType.ModalSubmit"/> Interaction.
    /// </summary>
    public class RestModalData : IModalInteractionData
    {
        /// <inheritdoc/>
        public string CustomId { get; }

        /// <summary>
        ///     Represents the <see cref="Modal"/>s components submitted by the user.
        /// </summary>
        public IReadOnlyCollection<RestMessageComponentData> Components { get; }

        IReadOnlyCollection<IComponentInteractionData> IModalInteractionData.Components => Components;

        internal RestModalData(Model model, BaseDiscordClient discord, IGuild guild)
        {
            CustomId = model.CustomId;
            Components = model.Components
                .SelectMany(x => x.Components)
                .Select(x => new RestMessageComponentData(x, discord, guild))
                .ToArray();
        }
    }
}
