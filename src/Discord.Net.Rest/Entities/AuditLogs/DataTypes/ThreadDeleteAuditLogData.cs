using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a thread deletion.
    /// </summary>
    public class ThreadDeleteAuditLogData : IAuditLogData
    {
        private ThreadDeleteAuditLogData(ulong id, string name, ThreadType type, bool archived,
            ThreadArchiveDuration autoArchiveDuration, bool locked, int? rateLimit)
        {
            ThreadId = id;
            ThreadName = name;
            ThreadType = type;
            IsArchived = archived;
            AutoArchiveDuration = autoArchiveDuration;
            IsLocked = locked;
            SlowModeInterval = rateLimit;
        }

        internal static ThreadDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var id = entry.TargetId.Value;
            var thread = log.Threads.FirstOrDefault(x => x.Id == id);

            var nameModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var typeModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "type");

            var archivedModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "archived");
            var autoArchiveDurationModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "auto_archive_duration");
            var lockedModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "locked");
            var rateLimitPerUserModel = changes.FirstOrDefault(x => x.ChangedProperty == "rate_limit_per_user");

            var name = nameModel.OldValue.ToObject<string>(discord.ApiClient.Serializer);
            var type = typeModel.OldValue.ToObject<ThreadType>(discord.ApiClient.Serializer);

            var archived = archivedModel.OldValue.ToObject<bool>(discord.ApiClient.Serializer);
            var autoArchiveDuration = autoArchiveDurationModel.OldValue.ToObject<ThreadArchiveDuration>(discord.ApiClient.Serializer);
            var locked = lockedModel.OldValue.ToObject<bool>(discord.ApiClient.Serializer);
            var rateLimit = rateLimitPerUserModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);

            return new ThreadDeleteAuditLogData(id, name, type, archived, autoArchiveDuration, locked, rateLimit);
        }

        /// <summary>
        ///     Gets the snowflake ID of the deleted thread.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the deleted thread.
        /// </returns>
        public ulong ThreadId { get; }
        /// <summary>
        ///     Gets the name of the deleted thread.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the deleted thread.
        /// </returns>
        public string ThreadName { get; }
        /// <summary>
        ///     Gets the type of the deleted thread.
        /// </summary>
        /// <returns>
        ///     The type of thread that was deleted.
        /// </returns>
        public ThreadType ThreadType { get; }
        /// <summary>
        ///     Gets the value that indicates whether the deleted thread was archived.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this thread had the Archived flag enabled; otherwise <c>false</c>.
        /// </returns>
        public bool IsArchived { get; }
        /// <summary>
        ///     Gets the thread auto archive duration of the deleted thread.
        /// </summary>
        /// <returns>
        ///     The thread auto archive duration of the thread that was deleted.
        /// </returns>
        public ThreadArchiveDuration AutoArchiveDuration { get; }
        /// <summary>
        ///     Gets the value that indicates whether the deleted thread was locked.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this thread had the Locked flag enabled; otherwise <c>false</c>.
        /// </returns>
        public bool IsLocked { get; }
        /// <summary>
        ///     Gets the slow-mode delay of the deleted thread.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the time in seconds required before the user can send another
        ///     message; <c>0</c> if disabled.
        ///     <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public int? SlowModeInterval { get; }
    }
}
