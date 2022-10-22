using System.Collections.Generic;
using System.Linq;
using System;
using Model = Discord.API.ModalInteractionData;
using InterationModel = Discord.API.Interaction;
using DataModel = Discord.API.MessageComponentInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents data sent from a <see cref="InteractionType.ModalSubmit"/> Interaction.
    /// </summary>
    public class RestModalData : IComponentInteractionData, IModalInteractionData
    {
        /// <inheritdoc/>
        public string CustomId { get; }

        /// <summary>
        ///     Represents the <see cref="Modal"/>s components submitted by the user.
        /// </summary>
        public IReadOnlyCollection<RestMessageComponentData> Components { get; }

        /// <inheritdoc/>
        public ComponentType Type => throw new NotSupportedException("Modals do not have a component type.");

        /// <inheritdoc/>
        public IReadOnlyCollection<string> Values
            => throw new NotSupportedException("Modal interactions do not have values!");

        /// <inheritdoc/>
        public IReadOnlyCollection<IChannel> Channels
            => throw new NotSupportedException("Modal interactions do not have channels!");

        /// <inheritdoc/>
        public IReadOnlyCollection<IUser> Users
            => throw new NotSupportedException("Modal interactions do not have users!");

        /// <inheritdoc/>
        public IReadOnlyCollection<IRole> Roles
            => throw new NotSupportedException("Modal interactions do not have roles!");

        /// <inheritdoc/>
        public string Value
            => throw new NotSupportedException("Modal interactions do not have value!");

        IReadOnlyCollection<IComponentInteractionData> IModalInteractionData.Components => Components;

        internal RestModalData(Model model)
        {
            CustomId = model.CustomId;
            Components = model.Components
                .SelectMany(x => x.Components)
                .Select(x => new RestMessageComponentData(x))
                .ToArray();
        }
    }
}
