using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a select menu component defined at <see href="https://discord.com/developers/docs/interactions/message-components#select-menu-object"/>
    /// </summary>
    public class SelectMenuComponent : IMessageComponent
    {
        /// <inheritdoc/>
        public ComponentType Type => ComponentType.SelectMenu;

        /// <summary>
        ///     The custom id of this Select menu that will be sent with a <see cref="IDiscordInteraction"/>.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     The menus options to select from.
        /// </summary>
        public IReadOnlyCollection<SelectMenuOption> Options { get; }

        /// <summary>
        ///     A custom placeholder text if nothing is selected, max 100 characters.
        /// </summary>
        public string Placeholder { get; }

        /// <summary>
        ///     The minimum number of items that must be chosen; default 1, min 0, max 25
        /// </summary>
        public int MinValues { get; }

        /// <summary>
        ///     The maximum number of items that can be chosen; default 1, max 25
        /// </summary>
        public int MaxValues { get; }

        /// <summary>
        ///     Whether this menu is disabled or not.
        /// </summary>
        public bool Disabled { get; }

        /// <summary>
        ///     Turns this select menu into a builder.
        /// </summary>
        /// <returns>
        ///     A newly create builder with the same properties as this select menu.
        /// </returns>
        public SelectMenuBuilder ToBuilder()
            => new SelectMenuBuilder(
                this.CustomId,
                this.Options.Select(x => new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, x.Default)).ToList(),
                this.Placeholder,
                this.MaxValues,
                this.MinValues,
                this.Disabled);

        internal SelectMenuComponent(string customId, List<SelectMenuOption> options, string placeholder, int minValues, int maxValues, bool disabled)
        {
            this.CustomId = customId;
            this.Options = options;
            this.Placeholder = placeholder;
            this.MinValues = minValues;
            this.MaxValues = maxValues;
            this.Disabled = disabled;
        }
    }
}
