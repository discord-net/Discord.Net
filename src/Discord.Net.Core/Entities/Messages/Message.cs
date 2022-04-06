using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a message created by a <see cref="MessageBuilder"/> that can be sent to a channel. 
    /// </summary>
    public sealed class Message
    {
        /// <summary>
        ///     Gets the content of the message.
        /// </summary>
        public string Content { get; }

        /// <summary>
        ///     Gets whether or not this message should be read by a text-to-speech engine.
        /// </summary>
        public bool IsTTS { get; }

        /// <summary>
        ///     Gets a collection of embeds sent along with this message.
        /// </summary>
        public IReadOnlyCollection<Embed> Embeds { get; }

        /// <summary>
        ///     Gets the allowed mentions for this message.
        /// </summary>
        public AllowedMentions AllowedMentions { get; }

        /// <summary>
        ///     Gets the message reference (reply to) for this message.
        /// </summary>
        public MessageReference MessageReference { get; }

        /// <summary>
        ///     Gets the components of this message.
        /// </summary>
        public MessageComponent Components { get; }

        /// <summary>
        ///     Gets a collection of sticker ids that will be sent with this message.
        /// </summary>
        public IReadOnlyCollection<ulong> StickerIds { get; }

        /// <summary>
        ///     Gets a collection of files sent with this message.
        /// </summary>
        public IReadOnlyCollection<FileAttachment> Attachments { get; }

        /// <summary>
        ///     Gets the message flags for this message.
        /// </summary>
        public MessageFlags Flags { get; }

        internal Message(string content, bool istts, IReadOnlyCollection<Embed> embeds, AllowedMentions allowedMentions,
            MessageReference messagereference, MessageComponent components, IReadOnlyCollection<ulong> stickerIds,
            IReadOnlyCollection<FileAttachment> attachments, MessageFlags flags)
        {
            Content = content;
            IsTTS = istts;
            Embeds = embeds;
            AllowedMentions = allowedMentions;
            MessageReference = messagereference;
            Components = components;
            StickerIds = stickerIds;
            Attachments = attachments;
            Flags = flags;
        }
    }
}
