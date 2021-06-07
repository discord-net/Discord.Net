using System;

namespace Discord.Net.Models
{
    /// <summary>
    /// Declares a flag enum which represents the message flags for a <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#message-object-message-flags"/>
    /// </remarks>
    [Flags]
    public enum MessageFlags
    {
        /// <summary>
        /// This <see cref="Message"/> has been published to subscribed
        /// <see cref="Channel"/>s (via Channel Following).
        /// </summary>
        Crossposted = 1 << 0,

        /// <summary>
        /// This <see cref="Message"/> originated from a <see cref="Message"/>
        /// in another <see cref="Channel"/> (via Channel Following).
        /// </summary>
        IsCrosspost = 1 << 1,

        /// <summary>
        /// Do not include any <see cref="Embed"/>s when serializing this <see cref="Message"/>.
        /// </summary>
        SuppressEmbeds = 1 << 2,

        /// <summary>
        /// The source <see cref="Message"/> for this crosspost has been
        /// deleted (via Channel Following).
        /// </summary>
        SourceMessageDeleted = 1 << 3,

        /// <summary>
        /// This <see cref="Message"/> came from the urgent message system.
        /// </summary>
        Urgent = 1 << 4,

        /// <summary>
        /// This <see cref="Message"/> has an associated <see cref="ThreadMetadata"/>,
        /// with the same id as the message.
        /// </summary>
        HasThread = 1 << 5,

        /// <summary>
        /// This <see cref="Message"/> is only visible to the user who invoked the Interaction.
        /// </summary>
        Ephemeral = 1 << 6,

        /// <summary>
        /// This <see cref="Message"/> is an Interaction Response and the bot is "thinking".
        /// </summary>
        Loading = 1 << 7,
    }
}
