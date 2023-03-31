using System;

namespace Discord
{
    [Flags]
    public enum MessageFlags
    {
        /// <summary>
        ///     Default value for flags, when none are given to a message.
        /// </summary>
        None = 0,
        /// <summary>
        ///     Flag given to messages that have been published to subscribed
        ///     channels (via Channel Following).
        /// </summary>
        Crossposted = 1 << 0,
        /// <summary>
        ///     Flag given to messages that originated from a message in another
        ///     channel (via Channel Following).
        /// </summary>
        IsCrosspost = 1 << 1,
        /// <summary>
        ///     Flag given to messages that do not display any embeds.
        /// </summary>
        SuppressEmbeds = 1 << 2,
        /// <summary>
        ///     Flag given to messages that the source message for this crosspost
        ///     has been deleted (via Channel Following).
        /// </summary>
        SourceMessageDeleted = 1 << 3,
        /// <summary>
        ///     Flag given to messages that came from the urgent message system.
        /// </summary>
        Urgent = 1 << 4,
        /// <summary>
        ///     Flag given to messages has an associated thread, with the same id as the message
        /// </summary>
        HasThread = 1 << 5,
        /// <summary>
        ///     Flag given to messages that is only visible to the user who invoked the Interaction.
        /// </summary>
        Ephemeral = 1 << 6,
        /// <summary>
        ///     Flag given to messages that is an Interaction Response and the bot is "thinking"
        /// </summary>
        Loading = 1 << 7,
        /// <summary>
        ///     Flag given to messages that failed to mention some roles and add their members to the thread.
        /// </summary>
        FailedToMentionRolesInThread = 1 << 8,
        /// <summary>
        ///     Flag give to messages that will not trigger push and desktop notifications.
        /// </summary>
        SuppressNotification = 1 << 12
    }
}
