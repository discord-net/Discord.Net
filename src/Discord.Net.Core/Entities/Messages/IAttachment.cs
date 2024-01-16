using System;
using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a message attachment found in a <see cref="IUserMessage"/>.
    /// </summary>
    public interface IAttachment : ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the filename of this attachment.
        /// </summary>
        /// <returns>
        ///     A string containing the full filename of this attachment (e.g. <c>textFile.txt</c>).
        /// </returns>
        string Filename { get; }
        /// <summary>
        ///     Gets the URL of this attachment.
        /// </summary>
        /// <returns>
        ///     A string containing the URL of this attachment.
        /// </returns>
        string Url { get; }
        /// <summary>
        ///     Gets a proxied URL of this attachment.
        /// </summary>
        /// <returns>
        ///     A string containing the proxied URL of this attachment.
        /// </returns>
        string ProxyUrl { get; }
        /// <summary>
        ///     Gets the file size of this attachment.
        /// </summary>
        /// <returns>
        ///     The size of this attachment in bytes.
        /// </returns>
        int Size { get; }
        /// <summary>
        ///     Gets the height of this attachment.
        /// </summary>
        /// <returns>
        ///     The height of this attachment if it is a picture; otherwise <see langword="null" />.
        /// </returns>
        int? Height { get; }
        /// <summary>
        ///     Gets the width of this attachment.
        /// </summary>
        /// <returns>
        ///     The width of this attachment if it is a picture; otherwise <see langword="null" />.
        /// </returns>
        int? Width { get; }
        /// <summary>
        ///     Gets whether or not this attachment is ephemeral.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the attachment is ephemeral; otherwise <see langword="false"/>.
        /// </returns>
        bool Ephemeral { get; }
        /// <summary>
        ///     Gets the description of the attachment; or <see langword="null"/> if there is none set.
        /// </summary>
        string Description { get; }
        /// <summary>
        ///     Gets the media's <see href="https://en.wikipedia.org/wiki/Media_type">MIME type</see> if present; otherwise <see langword="null"/>.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        ///     Gets the duration of the audio file. <see langword="null"/> if the attachment is not a voice message.
        /// </summary>
        double? Duration { get; }

        /// <summary>
        ///     Gets the base64 encoded bytearray representing a sampled waveform. <see langword="null"/> if the attachment is not a voice message.
        /// </summary>
        public string Waveform { get; }

        /// <summary>
        ///     Gets flags related to this to this attachment.
        /// </summary>
        public AttachmentFlags Flags { get; }

        /// <summary>
        ///     Gets users who participated in the clip.
        /// </summary>
        public IReadOnlyCollection<IUser> ClipParticipants { get; }

        /// <summary>
        ///     Gets the title of the clip. <see langword="null"/> if the clip has no title set.
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     Gets the timestamp of the clip. <see langword="null"/> if the attachment is not a clip.
        /// </summary>
        public DateTimeOffset? ClipCreatedAt { get; }
    }
}
