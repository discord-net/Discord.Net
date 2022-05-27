using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a class that contains data present in all interactions to evaluate against at rest-interaction creation.
    /// </summary>
    public readonly struct InteractionProperties
    {
        /// <summary>
        ///     The type of this interaction.
        /// </summary>
        public InteractionType Type { get; }

        /// <summary>
        ///     Gets the type of application command this interaction represents.
        /// </summary>
        /// <remarks>
        ///     This will be <see langword="null"/> if the <see cref="Type"/> is not <see cref="InteractionType.ApplicationCommand"/>.
        /// </remarks>
        public ApplicationCommandType? CommandType { get; }

        /// <summary>
        ///     Gets the name of the interaction.
        /// </summary>
        /// <remarks>
        ///     This will be <see cref="string.Empty"/> if the <see cref="Type"/> is not <see cref="InteractionType.ApplicationCommand"/>.
        /// </remarks>
        public string Name { get; } = string.Empty;

        /// <summary>
        ///     Gets the custom ID of the interaction.
        /// </summary>
        /// <remarks>
        ///     This will be <see cref="string.Empty"/> if the <see cref="Type"/> is not <see cref="InteractionType.MessageComponent"/> or <see cref="InteractionType.ModalSubmit"/>.
        /// </remarks>
        public string CustomId { get; } = string.Empty;

        /// <summary>
        ///     Gets the guild ID of the interaction.
        /// </summary>
        /// <remarks>
        ///     This will be <see langword="null"/> if this interaction was not executed in a guild.
        /// </remarks>
        public ulong? GuildId { get; }

        /// <summary>
        ///     Gets the channel ID of the interaction.
        /// </summary>
        /// <remarks>
        ///     This will be <see langword="null"/> if this interaction is <see cref="InteractionType.Ping"/>.
        /// </remarks>
        public ulong? ChannelId { get; }

        internal InteractionProperties(API.Interaction model)
        {
            Type = model.Type;
            CommandType = null;

            if (model.GuildId.IsSpecified)
                GuildId = model.GuildId.Value;
            else
                GuildId = null;

            if (model.ChannelId.IsSpecified)
                ChannelId = model.ChannelId.Value;
            else
                ChannelId = null;

            switch (Type)
            {
                case InteractionType.ApplicationCommand:
                    {
                        var data = (API.ApplicationCommandInteractionData)model.Data;

                        CommandType = data.Type;
                        Name = data.Name;
                    }
                    break;
                case InteractionType.MessageComponent:
                    {
                        var data = (API.MessageComponentInteractionData)model.Data;

                        CustomId = data.CustomId;
                    }
                    break;
                case InteractionType.ModalSubmit:
                    {
                        var data = (API.ModalInteractionData)model.Data;

                        CustomId = data.CustomId;
                    }
                    break;
            }
        }
    }
}
