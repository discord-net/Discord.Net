using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a <see cref="IMessageComponent"/> Button.
    /// </summary>
    public class ButtonComponent : IMessageComponent
    {
        /// <inheritdoc/>
        public ComponentType Type { get; } = ComponentType.Button;

        /// <summary>
        ///     The <see cref="ButtonStyle"/> of this button, example buttons with each style can be found <see href="https://discord.com/assets/7bb017ce52cfd6575e21c058feb3883b.png">Here</see>.
        /// </summary>
        public ButtonStyle Style { get; }

        /// <summary>
        ///     The label of the button, this is the text that is shown.
        /// </summary>
        public string Label { get; }

        /// <summary>
        ///     A <see cref="IEmote"/> that will be displayed with this button.
        /// </summary>
        public IEmote Emote { get; }

        /// <summary>
        ///     A unique id that will be sent with a <see cref="IDiscordInteraction"/>. This is how you know what button was pressed.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     A URL for a <see cref="ButtonStyle.Link"/> button. 
        /// </summary>
        /// <remarks>
        ///     You cannot have a button with a <b>URL</b> and a <b>CustomId</b>.
        /// </remarks>
        public string Url { get; }

        /// <summary>
        ///     Whether this button is disabled or not.
        /// </summary>
        public bool Disabled { get; }

        /// <summary>
        ///     Turns this button into a button builder.
        /// </summary>
        /// <returns>
        ///     A newly created button builder with the same properties as this button.
        /// </returns>
        public ButtonBuilder ToBuilder()
            => new ButtonBuilder(Label, CustomId, Style, Url, Emote, Disabled);

        internal ButtonComponent(ButtonStyle style, string label, IEmote emote, string customId, string url, bool disabled)
        {
            Style = style;
            Label = label;
            Emote = emote;
            CustomId = customId;
            Url = url;
            Disabled = disabled;
        }
    }
}
