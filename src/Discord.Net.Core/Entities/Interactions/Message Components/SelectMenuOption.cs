using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a choice for a <see cref="SelectMenu"/>.
    /// </summary>
    public class SelectMenuOption
    {
        /// <summary>
        ///     The user-facing name of the option, max 25 characters.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     The dev-define value of the option, max 100 characters.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     An additional description of the option, max 50 characters.
        /// </summary>
        public string Description { get; }

        /// <summary>
        ///     A <see cref="IEmote"/> that will be displayed with this menu option.
        /// </summary>
        public IEmote Emote { get; }

        /// <summary>
        ///     Will render this option as selected by default.
        /// </summary>
        public bool? Default { get; }

        internal SelectMenuOption(string label, string value, string description, IEmote emote, bool? defaultValue)
        {
            this.Label = label;
            this.Value = value;
            this.Description = description;
            this.Emote = emote;
            this.Default = defaultValue;
        }
    }
}
