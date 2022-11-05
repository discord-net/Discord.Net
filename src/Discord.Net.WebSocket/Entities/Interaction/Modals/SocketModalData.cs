using System.Collections.Generic;
using System.Linq;
using System;
using Model = Discord.API.ModalInteractionData;
using InterationModel = Discord.API.Interaction;
using DataModel = Discord.API.MessageComponentInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents data sent from a <see cref="InteractionType.ModalSubmit"/>.
    /// </summary>
    public class SocketModalData : IModalInteractionData
    {
        /// <summary>
        ///     Gets the <see cref="Modal"/>'s Custom Id.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the <see cref="Modal"/>'s components submitted by the user.
        /// </summary>
        public IReadOnlyCollection<SocketMessageComponentData> Components { get; }

        internal SocketModalData(Model model, DiscordSocketClient discord, ClientState state, SocketGuild guild, API.User dmUser)
        {
            CustomId = model.CustomId;
            Components = model.Components
                .SelectMany(x => x.Components)
                .Select(x => new SocketMessageComponentData(x, discord, state, guild, dmUser))
                .ToArray();
        }

        IReadOnlyCollection<IComponentInteractionData> IModalInteractionData.Components => Components;
    }
}
