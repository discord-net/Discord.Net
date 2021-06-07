using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    /// Represents a discord thread metadata object.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#thread-metadata-object-thread-metadata-structure"/>
    /// </remarks>
    public record ThreadMetadata
    {
        /// <summary>
        /// Whether the thread is archived.
        /// </summary>
        [JsonPropertyName("archived")]
        public bool Archived { get; init; }

        /// <summary>
        /// Id of the <see cref="User"/> that last archived or unarchived the thread.
        /// </summary>
        [JsonPropertyName("archiver_id")]
        public Optional<Snowflake> ArchiverId { get; init; }

        /// <summary>
        /// Duration in minutes to automatically archive the thread after
        /// recent activity, can be set to: 60, 1440, 4320, 10080.
        /// </summary>
        [JsonPropertyName("auto_archive_duration")]
        public int AutoArchiveDuration { get; init; }

        /// <summary>
        /// Timestamp when the thread's archive status was last changed, used for
        /// calculating recent activity.
        /// </summary>
        [JsonPropertyName("archive_timestamp")]
        public DateTimeOffset ArchiveTimestamp { get; init; }

        /// <summary>
        /// When a thread is locked, only <see cref="User"/>s with
        /// <see cref="Permissions.ManageThreads"/> can unarchive it.
        /// </summary>
        [JsonPropertyName("locked")]
        public Optional<bool> Locked { get; init; }
    }
}
