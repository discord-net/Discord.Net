using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
    /// <summary>
    ///     Represents a select menu component defined at <see href="https://discord.com/developers/docs/interactions/message-components#select-menu-object"/>
    /// </summary>
    public class SelectMenuComponent : IMessageComponent
    {
        /// <inheritdoc/>
        public ComponentType Type { get; }

        /// <inheritdoc/>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the menus options to select from.
        /// </summary>
        public IReadOnlyCollection<SelectMenuOption> Options { get; }

        /// <summary>
        ///     Gets the custom placeholder text if nothing is selected.
        /// </summary>
        public string Placeholder { get; }

        /// <summary>
        ///     Gets the minimum number of items that must be chosen.
        /// </summary>
        public int MinValues { get; }

        /// <summary>
        ///     Gets the maximum number of items that can be chosen.
        /// </summary>
        public int MaxValues { get; }

        /// <summary>
        ///     Gets whether this menu is disabled or not.
        /// </summary>
        public bool IsDisabled { get; }

        /// <summary>
        ///     Gets the allowed channel types for this modal
        /// </summary>
        public IReadOnlyCollection<ChannelType> ChannelTypes { get; }

        /// <summary>
        ///     Turns this select menu into a builder.
        /// </summary>
        /// <returns>
        ///     A newly create builder with the same properties as this select menu.
        /// </returns>
        public SelectMenuBuilder ToBuilder()
            => new SelectMenuBuilder(
                CustomId,
                Options.Select(x => new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, x.IsDefault)).ToList(),
                Placeholder,
                MaxValues,
                MinValues,
                IsDisabled, Type, ChannelTypes.ToList());

        internal SelectMenuComponent(string customId, List<SelectMenuOption> options, string placeholder, int minValues, int maxValues, bool disabled, ComponentType type, IEnumerable<ChannelType> channelTypes = null)
        {
            CustomId = customId;
            Options = options;
            Placeholder = placeholder;
            MinValues = minValues;
            MaxValues = maxValues;
            IsDisabled = disabled;
            Type = type;
            ChannelTypes = channelTypes?.ToArray() ?? Array.Empty<ChannelType>();
        }
    }
}
