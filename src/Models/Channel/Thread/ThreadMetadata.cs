using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a thread metadata object.
    /// </summary>
    public record ThreadMetadata
    {
        /// <summary>
        ///     Creates a <see cref="ThreadMetadata"/> with the provided parameters.
        /// </summary>
        /// <param name="archived">Whether the thread is archived.</param>
        /// <param name="archiverId">Id of the user that last archived or unarchived the thread.</param>
        /// <param name="autoArchiveDuration">Duration in minutes to automatically archive the thread after recent activity, can be set to: 60, 1440, 4320, 10080.</param>
        /// <param name="archiveTimestamp">Timestamp when the thread's archive status was last changed, used for calculating recent activity.</param>
        /// <param name="locked">When a thread is locked, only users with MANAGE_THREADS can unarchive it.</param>
        [JsonConstructor]
        public ThreadMetadata(bool archived, Optional<Snowflake> archiverId, int autoArchiveDuration, DateTimeOffset archiveTimestamp, Optional<bool> locked)
        {
            Archived = archived;
            ArchiverId = archiverId;
            AutoArchiveDuration = autoArchiveDuration;
            ArchiveTimestamp = archiveTimestamp;
            Locked = locked;
        }

        /// <summary>
        ///     Whether the thread is archived.
        /// </summary>
        [JsonPropertyName("archived")]
        public bool Archived { get; }

        /// <summary>
        ///     Id of the user that last archived or unarchived the thread.
        /// </summary>
        [JsonPropertyName("archiver_id")]
        public Optional<Snowflake> ArchiverId { get; }

        /// <summary>
        ///     Duration in minutes to automatically archive the thread after recent activity, can be set to: 60, 1440, 4320, 10080.
        /// </summary>
        [JsonPropertyName("auto_archive_duration")]
        public int AutoArchiveDuration { get; }

        /// <summary>
        ///     Timestamp when the thread's archive status was last changed, used for calculating recent activity.
        /// </summary>
        [JsonPropertyName("archive_timestamp")]
        public DateTimeOffset ArchiveTimestamp { get; }

        /// <summary>
        ///     When a thread is locked, only users with MANAGE_THREADS can unarchive it.
        /// </summary>
        [JsonPropertyName("locked")]
        public Optional<bool> Locked { get; }
    }
}
